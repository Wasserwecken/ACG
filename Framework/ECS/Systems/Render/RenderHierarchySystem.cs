using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Materials;
using Framework.Assets.Verticies;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Scene;
using Framework.ECS.Components.Transform;
using System.Collections.Generic;

namespace Framework.ECS.Systems.Hierarchy
{
    [With(typeof(MeshComponent))]
    [With(typeof(TransformComponent))]
    public class RenderHierarchySystem : AEntitySetSystem<bool>
    {
        private readonly RenderDataComponent _renderData;

        /// <summary>
        /// 
        /// </summary>
        public RenderHierarchySystem(World world, Entity worldComponents) : base(world)
        {
            _renderData = worldComponents.Get<RenderDataComponent>();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void PreUpdate(bool state)
        {
            _renderData.Graph.Clear();
            _renderData.Shaders.Clear();
            _renderData.Materials.Clear();
            _renderData.Transforms.Clear();
            _renderData.Primitves.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, in Entity entity)
        {
            var mesh = entity.Get<MeshComponent>();
            var transform = entity.Get<TransformComponent>();

            for (int i = 0; i < mesh.Mesh.Primitives.Count; i++)
            {
                var shader = mesh.Shaders[mesh.Shaders.Count > i ? i : 0];
                var material = mesh.Materials[mesh.Materials.Count > i ? i : 0];
                var primitive = mesh.Mesh.Primitives[i];

                if (!_renderData.Graph.ContainsKey(shader))
                {
                    _renderData.Graph.Add(shader, new Dictionary<MaterialAsset, Dictionary<TransformComponent, List<VertexPrimitiveAsset>>>());
                    _renderData.Shaders.Add(shader);
                }

                if (!_renderData.Graph[shader].ContainsKey(material))
                {
                    _renderData.Graph[shader].Add(material, new Dictionary<TransformComponent, List<VertexPrimitiveAsset>>());
                    _renderData.Materials.Add(material);
                }

                if (!_renderData.Graph[shader][material].ContainsKey(transform))
                {
                    _renderData.Graph[shader][material].Add(transform, new List<VertexPrimitiveAsset>());
                    _renderData.Transforms.Add(transform);
                }

                _renderData.Graph[shader][material][transform].Add(primitive);
                _renderData.Primitves.Add(primitive);
            }
        }
    }
}
