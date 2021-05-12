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
    [With(typeof(PointLightComponent))]
    [With(typeof(PointShadowComponent))]
    public class PointShadowPassSystem : AEntitySetSystem<bool>
    {
        private readonly Entity _worldComponents;
        private readonly EntitySet _renderCandidates;
        private readonly ShaderPointShadowBlock _shadowBlock;
        private readonly ShaderViewSpaceBlock _viewBlock;

        /// <summary>
        /// 
        /// </summary>
        public PointShadowPassSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
            _shadowBlock = new ShaderPointShadowBlock();
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
            _shadowBlock.Shadows = new ShaderPointShadowBlock.ShaderPointShadow[lightCount];
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, ReadOnlySpan<Entity> entities)
        {
            // GLOBAL PREPERATION
            ref var shaderInfo = ref _worldComponents.Get<PointLightBufferComponent>();
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
                var lightConfig = entity.Get<PointLightComponent>();
                var shadowConfig = entity.Get<PointShadowComponent>();

                if (shadowConfig.Strength > float.Epsilon && shaderInfo.ShadowSpacer.Add(shadowConfig.Resolution, out var shadowMapSpace))
                {
                    // SHADOW DATA
                    var foo = (shadowConfig.Resolution / 3f) % (shadowConfig.Resolution / 3) > 0.5f ? 2f : 1f;
                    var widthCorrection = (shadowConfig.Resolution - foo) / shadowConfig.Resolution;
                    _shadowBlock.Shadows[lightConfig.InfoId].Area = new Vector4(shadowMapSpace, shadowMapSpace.Z * widthCorrection);
                    _shadowBlock.Shadows[lightConfig.InfoId].Strength = new Vector4(shadowConfig.Strength, shadowConfig.NearClipping, 0f, 0f);

                    // RENDER 6 SIDES
                    var cubeOrientations = Helper.CreateCubeOrientations(transform.Position);
                    for (int i = 0; i < cubeOrientations.Length; i++)
                    {
                        // VIEWPORT PREPERATION
                        var viewPort = shadowMapSpace * new Vector3(
                            shaderInfo.ShadowBuffer.Width,
                            shaderInfo.ShadowBuffer.Height,
                            shaderInfo.ShadowBuffer.Width);
                        var width = (int)viewPort.Z / 3;
                        var height = (int)viewPort.Z / 2;
                        var x = (int)viewPort.X + (i % 3) * width;
                        var y = (int)viewPort.Y + (i < 3 ? 0 : height);
                        GL.Viewport(x, y, width, height);

                        // VIEW SPACE SETUP
                        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90f), 1f, shadowConfig.NearClipping, lightConfig.Range);
                        _viewBlock.WorldToView = cubeOrientations[i];
                        _viewBlock.WorldToProjection = cubeOrientations[i] * projection;
                        _viewBlock.WorldToViewRotation = cubeOrientations[i].ClearScale().ClearTranslation();
                        _viewBlock.WorldToProjectionRotation = cubeOrientations[i].ClearScale().ClearTranslation() * projection;
                        _viewBlock.ViewPosition = new Vector4(transform.Position, 1);
                        _viewBlock.ViewDirection = new Vector4(cubeOrientations[i].Row2);
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
