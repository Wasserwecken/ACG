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
        private readonly Dictionary<ShaderProgramAsset, Dictionary<MaterialAsset, Dictionary<TransformComponent, List<VertexPrimitiveAsset>>>> _renderGraph;
        private readonly Entity _worldComponents;
        protected readonly EntitySet _renderCandidates;
        private readonly ShaderBlockArray<ShaderDirectionalLight> _directionalInfoBlock;
        
        /// <summary>
        /// 
        /// </summary>
        public DirectionalShadowPassSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
            _renderGraph = new Dictionary<ShaderProgramAsset, Dictionary<MaterialAsset, Dictionary<TransformComponent, List<VertexPrimitiveAsset>>>>();
            _directionalInfoBlock = new ShaderBlockArray<ShaderDirectionalLight>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
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
            ref var shaderInfo = ref _worldComponents.Get<DirectionalLightCollectionComponent>();
            shaderInfo.ShadowSpacer.Clear();

            Renderer.UseFrameBuffer(shaderInfo.ShadowBuffer);
            GL.ClearColor(shaderInfo.ShadowBuffer.ClearColor);
            GL.Clear(shaderInfo.ShadowBuffer.ClearMask);

            foreach (ref readonly var entity in entities)
            {
                // DATA COLLECTION
                var transform = entity.Get<TransformComponent>();
                var lightConfig = entity.Get<DirectionalLightComponent>();
                var shadowConfig = entity.Get<DirectionalShadowComponent>();
                var viewSpace = CreateViewSpace(shadowConfig, transform);
                
                if (shadowConfig.Strength > float.Epsilon)
                {
                    // DATA PREPERATION
                    shaderInfo.ShadowSpacer.Add(shadowConfig.Resolution, out var shadowMapSpace);
                    shaderInfo.Data[lightConfig.InfoId].ShadowArea = new Vector4(shadowMapSpace, shadowMapSpace.Z);
                    shaderInfo.Data[lightConfig.InfoId].ShadowSpace = viewSpace.WorldToProjection;
                    shaderInfo.Data[lightConfig.InfoId].ShadowStrength = new Vector4(shadowConfig.Strength, 0f, 0f, 0f);

                    // BUILD RENDER GRAPH
                    _renderGraph.Clear();
                    foreach (ref readonly var candidate in _renderCandidates.GetEntities())
                    {
                        var candidatePrimitive = candidate.Get<PrimitiveComponent>();
                        var candidateTransform = candidate.Get<TransformComponent>();

                        if (candidatePrimitive.IsShadowCaster)
                            AddToGraph(
                                Defaults.Shader.Program.Shadow,
                                Defaults.Material.Shadow,
                                candidateTransform,
                                candidatePrimitive.Primitive
                            );
                    }

                    // VIEWPORT PREPERATION
                    var viewPort = shadowMapSpace * new Vector3(
                        shaderInfo.ShadowBuffer.Width,
                        shaderInfo.ShadowBuffer.Height,
                        shaderInfo.ShadowBuffer.Width);
                    GL.Viewport((int)viewPort.X, (int)viewPort.Y, (int)viewPort.Z, (int)viewPort.Z);

                    // DRAW RENDER GRAPH
                    ShaderBlockSingle<ShaderViewSpace>.Instance.Data = viewSpace;
                    ShaderBlockSingle<ShaderViewSpace>.Instance.PushToGPU();

                    foreach (var shaderRelation in _renderGraph)
                    {
                        Renderer.UseShader(shaderRelation.Key);
                        foreach (var materialRelation in shaderRelation.Value)
                        {
                            Renderer.UseMaterial(materialRelation.Key, shaderRelation.Key);
                            foreach (var transformRelation in materialRelation.Value)
                            {
                                ShaderBlockSingle<ShaderPrimitiveSpace>.Instance.Data = Renderer.CreatePrimitiveSpace(transformRelation.Key, viewSpace);
                                ShaderBlockSingle<ShaderPrimitiveSpace>.Instance.PushToGPU();

                                foreach (var primitive in transformRelation.Value)
                                    Renderer.Draw(primitive);
                            }
                        }
                    }
                }
            }

            // PUSH DATA TO GPU
            _directionalInfoBlock.Data = shaderInfo.Data;
            _directionalInfoBlock.PushToGPU();
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

        /// <summary>
        /// 
        /// </summary>
        protected virtual void AddToGraph(ShaderProgramAsset shader, MaterialAsset material, TransformComponent transform, VertexPrimitiveAsset verticies)
        {
            if (!_renderGraph.ContainsKey(shader))
                _renderGraph.Add(shader, new Dictionary<MaterialAsset, Dictionary<TransformComponent, List<VertexPrimitiveAsset>>>());

            if (!_renderGraph[shader].ContainsKey(material))
                _renderGraph[shader].Add(material, new Dictionary<TransformComponent, List<VertexPrimitiveAsset>>());

            if (!_renderGraph[shader][material].ContainsKey(transform))
                _renderGraph[shader][material].Add(transform, new List<VertexPrimitiveAsset>());

            _renderGraph[shader][material][transform].Add(verticies);
        }
    }
}
