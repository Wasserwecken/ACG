using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Framebuffer;
using Framework.Assets.Materials;
using Framework.Assets.Shader;
using Framework.Assets.Shader.Block;
using Framework.Assets.Shader.Block.Data;
using Framework.Assets.Verticies;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Transform;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Linq;

namespace Framework.ECS.Systems.Render
{
    public abstract class RenderPassBaseSystem : AEntitySetSystem<bool>
    {
        protected readonly Entity _worldComponents;
        protected readonly EntitySet _renderCandidates;
        protected readonly RenderGraph _renderGraph;

        /// <summary>
        /// 
        /// </summary>
        public RenderPassBaseSystem(World world, Entity worldComponents) : base(world)
        {
            _renderCandidates = SelectRenderCandidates();
            _worldComponents = worldComponents;
            _renderGraph = new RenderGraph();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, in Entity entity)
        {
            // DATA PREPERATION
            var passData = ValidatePassData(entity);
            var viewSpace = CreateViewSpace(entity);

            // BUILD GRAPH GRAPH
            _renderGraph.Clear();
            foreach (ref readonly var canidate in _renderCandidates.GetEntities())
                if (!CullCandidate(canidate, viewSpace))
                    AddToGraph(canidate);

            // PREPARE FRAMEBUFFER
            Renderer.UseFrameBuffer(passData.FrameBuffer);

            // DRAW GRAPH
            _renderGraph.Draw(viewSpace);
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual EntitySet SelectRenderCandidates() =>
            World.GetEntities()
                .With<TransformComponent>()
                .With<PrimitiveComponent>()
                .AsSet();

        /// <summary>
        /// 
        /// </summary>
        protected abstract RenderPassDataComponent ValidatePassData(Entity entity);

        /// <summary>
        /// 
        /// </summary>
        protected abstract ShaderViewSpace CreateViewSpace(Entity entity);

        /// <summary>
        /// 
        /// </summary>
        protected virtual bool CullCandidate(Entity candidate, ShaderViewSpace viewSpace) => false;

        /// <summary>
        /// 
        /// </summary>
        protected abstract void SelectRenderGraphData(Entity entity, out ShaderProgramAsset shader, out MaterialAsset material, out TransformComponent transform, out VertexPrimitiveAsset verticies);

        /// <summary>
        /// 
        /// </summary>
        protected virtual void AddToGraph(Entity entity)
        {
            SelectRenderGraphData(entity,
                out var shader,
                out var material,
                out var transform,
                out var verticies
            );

            if (!_renderGraph.ContainsKey(shader))
                _renderGraph.Add(shader, new Dictionary<MaterialAsset, Dictionary<TransformComponent, List<VertexPrimitiveAsset>>>());

            if (!_renderGraph[shader].ContainsKey(material))
                _renderGraph[shader].Add(material, new Dictionary<TransformComponent, List<VertexPrimitiveAsset>>());

            if (!_renderGraph[shader][material].ContainsKey(transform))
                _renderGraph[shader][material].Add(transform, new List<VertexPrimitiveAsset>());

            _renderGraph[shader][material][transform].Add(verticies);
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual ShaderPrimitiveSpace CreatePrimitiveSpace(TransformComponent primitiveTransform, ShaderViewSpace viewSpace)
        {
            return new ShaderPrimitiveSpace
            {
                LocalToWorld = primitiveTransform.WorldSpace,
                LocalToView = primitiveTransform.WorldSpace * viewSpace.WorldToViewInverse,
                LocalToProjection = primitiveTransform.WorldSpace * viewSpace.WorldToViewInverse * viewSpace.ViewProjection,

                LocalToWorldRotation = primitiveTransform.WorldSpace.ClearScale(),
                LocalToViewRotation = (primitiveTransform.WorldSpace * viewSpace.WorldToViewInverse).ClearScale().ClearTranslation(),
                LocalToProjectionRotation = (primitiveTransform.WorldSpace * viewSpace.WorldToViewInverse).ClearScale().ClearTranslation() * viewSpace.ViewProjection,
            };
        }
    }
}
