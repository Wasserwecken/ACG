using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Shader.Block;
using Framework.ECS.Components.Light;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Scene;
using Framework.ECS.Components.Transform;
using Framework.ECS.Systems.Render.OpenGL;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;

namespace Framework.ECS.Systems.Render.Pipeline
{
    [With(typeof(TransformComponent))]
    [With(typeof(DirectionalLightComponent))]
    [With(typeof(DirectionalShadowComponent))]
    public class DirectionalShadowPassSystem : AEntitySetSystem<bool>
    {
        private readonly Entity _worldComponents;
        protected readonly EntitySet _renderCandidates;

        /// <summary>
        /// 
        /// </summary>
        public DirectionalShadowPassSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
            _renderCandidates = World.GetEntities()
                .With<TransformComponent>()
                .With<PrimitiveComponent>()
                .AsSet();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void PreUpdate(bool state)
        {
            var lightCount = World.GetEntities().With<TransformComponent>().With<DirectionalLightComponent>().AsSet().Count;
            _worldComponents.Get<ShadowBufferComponent>().DirectionalBlock.Shadows = new ShaderDirectionalShadowBlock.ShaderDirectionalShadow[lightCount];
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, ReadOnlySpan<Entity> entities)
        {
            // GLOBAL PREPERATION
            ref var shadowBuffer = ref _worldComponents.Get<ShadowBufferComponent>();

            Renderer.Use(shadowBuffer.FramebufferBuffer);
            Renderer.Use(Defaults.Shader.Program.Shadow);
            Renderer.Use(Defaults.Material.Shadow, Defaults.Shader.Program.Shadow);

            GL.Enable(EnableCap.ScissorTest);
            GL.ClearColor(shadowBuffer.FramebufferBuffer.ClearColor);

            foreach (ref readonly var entity in entities)
            {
                // DATA COLLECTION
                var transform = entity.Get<TransformComponent>();
                var lightConfig = entity.Get<DirectionalLightComponent>();
                var shadowConfig = entity.Get<DirectionalShadowComponent>();

                if (shadowConfig.Strength > float.Epsilon && shadowBuffer.TextureAtlas.Add(shadowConfig.Resolution, out var shadowMapSpace))
                {
                    // VIEW SPACE SETUP
                    if (shadowConfig.ViewSpaceBlock == null)
                        shadowConfig.ViewSpaceBlock = new ShaderViewSpaceBlock();

                    var projection = Matrix4.CreateOrthographic(shadowConfig.Width, shadowConfig.Width, shadowConfig.NearClipping, shadowConfig.FarClipping);
                    transform.Forward = -transform.Forward;

                    shadowConfig.ViewSpaceBlock.WorldToView = transform.WorldSpaceInverse;
                    shadowConfig.ViewSpaceBlock.WorldToProjection = transform.WorldSpaceInverse * projection;
                    shadowConfig.ViewSpaceBlock.WorldToViewRotation = transform.WorldSpaceInverse.ClearScale().ClearTranslation();
                    shadowConfig.ViewSpaceBlock.WorldToProjectionRotation = transform.WorldSpaceInverse.ClearScale().ClearTranslation() * projection;
                    shadowConfig.ViewSpaceBlock.ViewPosition = new Vector4(transform.Position, 1);
                    shadowConfig.ViewSpaceBlock.ViewDirection = new Vector4(-transform.Forward, 0);
                    shadowConfig.ViewSpaceBlock.Resolution = new Vector2(shadowConfig.Resolution, shadowConfig.Resolution);
                    GPUSync.Push(shadowConfig.ViewSpaceBlock);
                    Renderer.Use(shadowConfig.ViewSpaceBlock, Defaults.Shader.Program.Shadow);

                    // SHADOW DATA
                    shadowBuffer.DirectionalBlock.Shadows[lightConfig.InfoId].Strength = new Vector4(shadowConfig.Strength, shadowConfig.NearClipping, shadowConfig.FarClipping, shadowConfig.Width);
                    shadowBuffer.DirectionalBlock.Shadows[lightConfig.InfoId].Area = new Vector4(shadowMapSpace, shadowMapSpace.Z);
                    shadowBuffer.DirectionalBlock.Shadows[lightConfig.InfoId].Space = shadowConfig.ViewSpaceBlock.WorldToProjection;

                    // VIEWPORT PREPERATION
                    var viewPort = shadowMapSpace * new Vector3(
                        shadowBuffer.FramebufferBuffer.Width,
                        shadowBuffer.FramebufferBuffer.Height,
                        shadowBuffer.FramebufferBuffer.Width);
                    GL.Viewport((int)viewPort.X, (int)viewPort.Y, (int)viewPort.Z, (int)viewPort.Z);
                    GL.Scissor((int)viewPort.X, (int)viewPort.Y, (int)viewPort.Z, (int)viewPort.Z);
                    GL.Clear(shadowBuffer.FramebufferBuffer.ClearMask);

                    // RENDER SHADOW CASTERS
                    foreach (ref readonly var candidate in _renderCandidates.GetEntities())
                    {
                        var primitive = candidate.Get<PrimitiveComponent>();
                        if (primitive.IsShadowCaster)
                        {
                            Renderer.Use(primitive.PrimitiveSpaceBlock, Defaults.Shader.Program.Shadow);
                            Renderer.Draw(primitive.Verticies);
                        }
                    }
                }
            }

            GL.Disable(EnableCap.ScissorTest);
        }
    }
}
