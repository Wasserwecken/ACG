using ACG.Framework.Assets;
using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Shader.Block;
using Framework.Assets.Shader.Block.Data;
using Framework.Assets.Shader.Info.Block.Data;
using Framework.ECS.Components.Light;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Transform;
using Framework.ECS.Systems.Render;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;

namespace Framework.ECS.Systems.RenderPipeline
{
    [With(typeof(TransformComponent))]
    [With(typeof(DirectionalLightComponent))]
    [With(typeof(DirectionalShadowComponent))]
    public class DirectionalShadowPassSystem : AEntitySetSystem<bool>
    {
        private readonly Entity _worldComponents;
        protected readonly EntitySet _renderCandidates;
        protected readonly ShaderDirectionalShadowBlock _shadowBlock;

        /// <summary>
        /// 
        /// </summary>
        public DirectionalShadowPassSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
            _shadowBlock = new ShaderDirectionalShadowBlock();
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
            _shadowBlock.Shadows = new ShaderDirectionalShadowBlock.ShaderDirectionalShadow[lightCount];
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, ReadOnlySpan<Entity> entities)
        {
            // GLOBAL PREPERATION
            ref var shaderInfo = ref _worldComponents.Get<DirectionalShadowBufferComponent>();

            shaderInfo.ShadowSpacer.Clear();

            Renderer.UseFrameBuffer(shaderInfo.ShadowBuffer);
            Renderer.UseShader(Defaults.Shader.Program.Shadow);
            Renderer.UseMaterial(Defaults.Material.Shadow, Defaults.Shader.Program.Shadow);

            GL.ClearColor(shaderInfo.ShadowBuffer.ClearColor);
            GL.Clear(shaderInfo.ShadowBuffer.ClearMask);

            foreach (ref readonly var entity in entities)
            {
                // DATA COLLECTION
                var transform = entity.Get<TransformComponent>();
                var lightConfig = entity.Get<DirectionalLightComponent>();
                var shadowConfig = entity.Get<DirectionalShadowComponent>();
                
                if (shadowConfig.Strength > float.Epsilon)
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
                    GPUSync.Push(shadowConfig.ViewSpaceBlock);
                    Renderer.UseShaderBlock(shadowConfig.ViewSpaceBlock, Defaults.Shader.Program.Shadow);

                    // SHADOW DATA
                    shaderInfo.ShadowSpacer.Add(shadowConfig.Resolution, out var shadowMapSpace);
                    _shadowBlock.Shadows[lightConfig.InfoId].Strength = new Vector4(shadowConfig.Strength, shadowConfig.NearClipping, shadowConfig.FarClipping, shadowConfig.Width);
                    _shadowBlock.Shadows[lightConfig.InfoId].Area = new Vector4(shadowMapSpace, shadowMapSpace.Z);
                    _shadowBlock.Shadows[lightConfig.InfoId].Space = shadowConfig.ViewSpaceBlock.WorldToProjection;

                    // VIEWPORT PREPERATION
                    var viewPort = shadowMapSpace * new Vector3(
                        shaderInfo.ShadowBuffer.Width,
                        shaderInfo.ShadowBuffer.Height,
                        shaderInfo.ShadowBuffer.Width);
                    GL.Viewport((int)viewPort.X, (int)viewPort.Y, (int)viewPort.Z, (int)viewPort.Z);

                    // RENDER SHADOW CASTERS
                    foreach (ref readonly var candidate in _renderCandidates.GetEntities())
                    {
                        var primitive = candidate.Get<PrimitiveComponent>();
                        if (primitive.IsShadowCaster)
                        {
                            Renderer.UseShaderBlock(primitive.ShaderSpaceBlock, Defaults.Shader.Program.Shadow);
                            Renderer.Draw(primitive.Verticies);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void PostUpdate(bool state)
        {
            GPUSync.Push(_shadowBlock);

            foreach (var shader in AssetRegister.Shaders)
                shader.SetBlockBinding(_shadowBlock);
        }
    }
}
