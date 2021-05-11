using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Materials;
using Framework.Assets.Shader;
using Framework.Assets.Shader.Block;
using Framework.Assets.Shader.Block.Data;
using Framework.Assets.Verticies;
using Framework.ECS.Components.Light;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Transform;
using Framework.ECS.Systems.Render;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace Framework.ECS.Systems.RenderPipeline
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
        protected override void Update(bool state, ReadOnlySpan<Entity> entities)
        {
            // GLOBAL PREPERATION
            ref var shaderInfo = ref _worldComponents.Get<DirectionalShadowBufferComponent>();
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
                var lightConfig = entity.Get<DirectionalLightComponent>();
                var shadowConfig = entity.Get<DirectionalShadowComponent>();
                
                if (shadowConfig.Strength > float.Epsilon)
                {
                    // VIEW SPACE SETUP
                    if (shadowConfig.ShaderViewSpace == null)
                        shadowConfig.ShaderViewSpace = new ShaderBlockSingle<ShaderViewSpace>(false, BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
                    shadowConfig.ShaderViewSpace.Data = CreateViewSpace(shadowConfig, transform);
                    shadowConfig.ShaderViewSpace.PushToGPU();

                    // SHADOW DATA
                    shaderInfo.ShadowSpacer.Add(shadowConfig.Resolution, out var shadowMapSpace);
                    shaderBlocks.DirectionalLights.Data[lightConfig.InfoId].ShadowArea = new Vector4(shadowMapSpace, shadowMapSpace.Z);
                    shaderBlocks.DirectionalLights.Data[lightConfig.InfoId].ShadowSpace = shadowConfig.ShaderViewSpace.Data.WorldToProjection;
                    shaderBlocks.DirectionalLights.Data[lightConfig.InfoId].ShadowStrength = new Vector4(shadowConfig.Strength, shadowConfig.NearClipping, shadowConfig.FarClipping, shadowConfig.Width);

                    // VIEWPORT PREPERATION
                    var viewPort = shadowMapSpace * new Vector3(
                        shaderInfo.ShadowBuffer.Width,
                        shaderInfo.ShadowBuffer.Height,
                        shaderInfo.ShadowBuffer.Width);
                    GL.Viewport((int)viewPort.X, (int)viewPort.Y, (int)viewPort.Z, (int)viewPort.Z);

                    // RENDER SHADOW CASTERS
                    Renderer.UseShaderBlock(shadowConfig.ShaderViewSpace, Defaults.Shader.Program.Shadow);
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

            shaderBlocks.DirectionalLights.PushToGPU();
        }

        /// <summary>
        /// 
        /// </summary>
        private ShaderViewSpace CreateViewSpace(DirectionalShadowComponent shadowConfig, TransformComponent transform)
        {
            var projection = Matrix4.CreateOrthographic(shadowConfig.Width, shadowConfig.Width, shadowConfig.NearClipping, shadowConfig.FarClipping);
            transform.Forward = -transform.Forward;

            return new ShaderViewSpace
            {
                WorldToView = transform.WorldSpaceInverse,
                WorldToProjection = transform.WorldSpaceInverse * projection,

                WorldToViewRotation = transform.WorldSpaceInverse.ClearScale().ClearTranslation(),
                WorldToProjectionRotation = transform.WorldSpaceInverse.ClearScale().ClearTranslation() * projection,

                ViewPosition = new Vector4(transform.Position, 1),
                ViewDirection = new Vector4(-transform.Forward, 0),
                ViewProjection = projection
            };
        }
    }
}
