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

namespace Framework.ECS.Systems.RenderPipeline
{
    [With(typeof(TransformComponent))]
    [With(typeof(PointLightComponent))]
    [With(typeof(PointShadowComponent))]
    public class PointShadowPassSystem : AEntitySetSystem<bool>
    {
        private readonly Entity _worldComponents;
        private readonly EntitySet _renderCandidates;
        private readonly ShaderViewSpaceBlock _viewBlock;

        /// <summary>
        /// 
        /// </summary>
        public PointShadowPassSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
            _viewBlock = new ShaderViewSpaceBlock();
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
            var lightCount = World.GetEntities().With<TransformComponent>().With<PointLightComponent>().AsSet().Count;
            _worldComponents.Get<ShadowBufferComponent>().PointBlock.Shadows = new ShaderPointShadowBlock.ShaderPointShadow[lightCount];
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, ReadOnlySpan<Entity> entities)
        {
            // GLOBAL PREPERATION
            ref var shadowBuffer = ref _worldComponents.Get<ShadowBufferComponent>();

            Renderer.UseFrameBuffer(shadowBuffer.FramebufferBuffer);
            Renderer.UseShader(Defaults.Shader.Program.Shadow);
            Renderer.UseMaterial(Defaults.Material.Shadow, Defaults.Shader.Program.Shadow);

            GL.Enable(EnableCap.ScissorTest);
            GL.ClearColor(shadowBuffer.FramebufferBuffer.ClearColor);

            foreach (ref readonly var entity in entities)
            {
                // DATA COLLECTION
                var transform = entity.Get<TransformComponent>();
                var lightConfig = entity.Get<PointLightComponent>();
                var shadowConfig = entity.Get<PointShadowComponent>();

                if (shadowConfig.Strength > float.Epsilon && shadowBuffer.TextureAtlas.Add(shadowConfig.Resolution, out var shadowMapSpace))
                {
                    // SHADOW DATA
                    var foo = (shadowConfig.Resolution / 3f) % (shadowConfig.Resolution / 3) > 0.5f ? 2f : 1f;
                    var widthCorrection = (shadowConfig.Resolution - foo) / shadowConfig.Resolution;
                    shadowBuffer.PointBlock.Shadows[lightConfig.InfoId].Area = new Vector4(shadowMapSpace, shadowMapSpace.Z * widthCorrection);
                    shadowBuffer.PointBlock.Shadows[lightConfig.InfoId].Strength = new Vector4(shadowConfig.Strength, shadowConfig.NearClipping, 0f, 0f);

                    // RENDER 6 SIDES
                    var cubeOrientations = Helper.CreateCubeOrientations(transform.Position);
                    for (int i = 0; i < cubeOrientations.Length; i++)
                    {
                        // VIEWPORT PREPERATION
                        var viewPort = shadowMapSpace * new Vector3(
                            shadowBuffer.FramebufferBuffer.Width,
                            shadowBuffer.FramebufferBuffer.Height,
                            shadowBuffer.FramebufferBuffer.Width);
                        var width = (int)viewPort.Z / 3;
                        var height = (int)viewPort.Z / 2;
                        var x = (int)viewPort.X + (i % 3) * width;
                        var y = (int)viewPort.Y + (i < 3 ? 0 : height);
                        GL.Viewport(x, y, width, height);
                        GL.Scissor(x, y, width, height);
                        GL.Clear(shadowBuffer.FramebufferBuffer.ClearMask);

                        // VIEW SPACE SETUP
                        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90f), 1f, shadowConfig.NearClipping, lightConfig.Range);
                        _viewBlock.WorldToView = cubeOrientations[i];
                        _viewBlock.WorldToProjection = cubeOrientations[i] * projection;
                        _viewBlock.WorldToViewRotation = cubeOrientations[i].ClearScale().ClearTranslation();
                        _viewBlock.WorldToProjectionRotation = cubeOrientations[i].ClearScale().ClearTranslation() * projection;
                        _viewBlock.ViewPosition = new Vector4(transform.Position, 1);
                        _viewBlock.ViewDirection = new Vector4(cubeOrientations[i].Row2);
                        _viewBlock.Resolution = new Vector2(shadowConfig.Resolution, shadowConfig.Resolution);
                        GPUSync.Push(_viewBlock);
                        Renderer.UseShaderBlock(_viewBlock, Defaults.Shader.Program.Shadow);


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

            GL.Disable(EnableCap.ScissorTest);
        }
    }
}
