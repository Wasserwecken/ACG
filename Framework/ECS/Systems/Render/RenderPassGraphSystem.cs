using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Materials;
using Framework.Assets.Verticies;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Transform;
using System.Collections.Generic;

namespace Framework.ECS.Systems.Render
{
    [With(typeof(RenderPassGraphComponent))]
    public class RenderPassGraphSystem : AEntitySetSystem<bool>
    {
        private readonly Entity _worldComponents;

        /// <summary>
        /// 
        /// </summary>
        public RenderPassGraphSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, in Entity entity)
        {
            ref var globalGPUData = ref _worldComponents.Get<GlobalGPUDataComponent>();
            ref var renderGraph = ref entity.Get<RenderPassGraphComponent>();

            renderGraph.Graph.Clear();

            foreach(var renderable in renderGraph.Renderables)
            {
                var primitive = renderable.Get<PrimitiveComponent>();
                var transform = renderable.Get<TransformComponent>();

                var shader = !entity.Has<RenderPassShaderComponent>() ? primitive.Shader : entity.Get<RenderPassShaderComponent>().Shader;
                var material = !entity.Has<RenderPassShaderComponent>() ? primitive.Material : entity.Get<RenderPassShaderComponent>().Material;
                var verticies = primitive.Primitive;

                if (!renderGraph.Graph.ContainsKey(shader))
                {
                    renderGraph.Graph.Add(shader, new Dictionary<MaterialAsset, Dictionary<TransformComponent, List<VertexPrimitiveAsset>>>());
                    if (!globalGPUData.Shaders.Contains(shader))
                        globalGPUData.Shaders.Add(shader);
                }

                if (!renderGraph.Graph[shader].ContainsKey(material))
                {
                    renderGraph.Graph[shader].Add(material, new Dictionary<TransformComponent, List<VertexPrimitiveAsset>>());
                    if (!globalGPUData.Materials.Contains(material))
                        globalGPUData.Materials.Add(material);
                }

                if (!renderGraph.Graph[shader][material].ContainsKey(transform))
                    renderGraph.Graph[shader][material].Add(transform, new List<VertexPrimitiveAsset>());

                renderGraph.Graph[shader][material][transform].Add(verticies);
                if (!globalGPUData.Primitives.Contains(verticies))
                    globalGPUData.Primitives.Add(verticies);
            }
        }
    }
}
