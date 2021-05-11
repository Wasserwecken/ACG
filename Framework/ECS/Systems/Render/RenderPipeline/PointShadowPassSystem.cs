using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Shader.Block;
using Framework.Assets.Shader.Block.Data;
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
        private readonly ShaderBlockSingle<ShaderViewSpace> _viewSpaceBlock;
        private readonly Entity _worldComponents;
        protected readonly EntitySet _renderCandidates;

        /// <summary>
        /// 
        /// </summary>
        public PointShadowPassSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
            _viewSpaceBlock = new ShaderBlockSingle<ShaderViewSpace>(false, BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
            _renderCandidates = World.GetEntities()
                .With<TransformComponent>()
                .With<PrimitiveComponent>()
                .AsSet();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, ReadOnlySpan<Entity> entities)
        {
            // GLOBAL PREPERATION
            ref var shaderInfo = ref _worldComponents.Get<PointLightBufferComponent>();
            ref var shaderBlocks = ref _worldComponents.Get<GlobalShaderBlocksComponent>();
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
                var viewSpaces = CreateViewSpace(shadowConfig, lightConfig, transform);

                if (shadowConfig.Strength > float.Epsilon && shaderInfo.ShadowSpacer.Add(shadowConfig.Resolution, out var shadowMapSpace))
                {
                    // SHADOW DATA
                    var foo = (shadowConfig.Resolution / 3f) % (shadowConfig.Resolution / 3) > 0.5f ? 2f : 1f;
                    var widthCorrection = (shadowConfig.Resolution - foo) / shadowConfig.Resolution;
                    shaderBlocks.PointLights.Data[lightConfig.InfoId].ShadowArea = new Vector4(shadowMapSpace, shadowMapSpace.Z * widthCorrection);
                    shaderBlocks.PointLights.Data[lightConfig.InfoId].ShadowStrength = new Vector4(shadowConfig.Strength, shadowConfig.NearClipping, 0f, 0f);

                    // RENDER 6 SIDES
                    for (int i = 0; i < viewSpaces.Length; i++)
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
                        _viewSpaceBlock.Data = viewSpaces[i];
                        _viewSpaceBlock.PushToGPU();

                        // RENDER SHADOW CASTERS
                        Renderer.UseShaderBlock(_viewSpaceBlock, Defaults.Shader.Program.Shadow);
                        foreach (ref readonly var candidate in _renderCandidates.GetEntities())
                        {
                            var candidatePrimitive = candidate.Get<PrimitiveComponent>();
                            if (candidatePrimitive.IsShadowCaster)
                            {
                                Renderer.UseShaderBlock(candidatePrimitive.ShaderSpace, Defaults.Shader.Program.Shadow);
                                Renderer.Draw(candidatePrimitive.Verticies);
                            }
                        }
                    }
                }
            }

            shaderBlocks.PointLights.PushToGPU();
        }

        /// <summary>
        /// 
        /// </summary>
        private ShaderViewSpace[] CreateViewSpace(PointShadowComponent shadowConfig, PointLightComponent lightConfig, TransformComponent transform)
        {
            var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90f), 1f, shadowConfig.NearClipping, lightConfig.Range);
            var orientations = new Matrix4[]
            {
                Matrix4.LookAt(transform.Position, transform.Position + Vector3.UnitZ, Vector3.UnitY),
                Matrix4.LookAt(transform.Position, transform.Position + Vector3.UnitY, Vector3.UnitX),
                Matrix4.LookAt(transform.Position, transform.Position + Vector3.UnitX, Vector3.UnitY),
                Matrix4.LookAt(transform.Position, transform.Position + -Vector3.UnitZ, Vector3.UnitY),
                Matrix4.LookAt(transform.Position, transform.Position + -Vector3.UnitY, -Vector3.UnitX),
                Matrix4.LookAt(transform.Position, transform.Position + -Vector3.UnitX, Vector3.UnitY)
            };

            var result = new ShaderViewSpace[orientations.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = new ShaderViewSpace
                {
                    WorldToView = orientations[i],
                    WorldToProjection = orientations[i] * projection,

                    WorldToViewRotation = orientations[i].ClearScale().ClearTranslation(),
                    WorldToProjectionRotation = orientations[i].ClearScale().ClearTranslation() * projection,

                    ViewPosition = new Vector4(transform.Position, 1),
                    ViewDirection = new Vector4(orientations[i].Row2),
                    ViewProjection = projection
                };

            return result;
        }
    }
}
