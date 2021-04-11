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
    [With(typeof(PrimitiveComponent))]
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
            var primitive = entity.Get<PrimitiveComponent>();
            var transform = entity.Get<TransformComponent>();

            if (!_renderData.Graph.ContainsKey(primitive.Shader))
            {
                _renderData.Graph.Add(primitive.Shader, new Dictionary<MaterialAsset, Dictionary<TransformComponent, List<VertexPrimitiveAsset>>>());
                _renderData.Shaders.Add(primitive.Shader);
            }

            if (!_renderData.Graph[primitive.Shader].ContainsKey(primitive.Material))
            {
                _renderData.Graph[primitive.Shader].Add(primitive.Material, new Dictionary<TransformComponent, List<VertexPrimitiveAsset>>());
                _renderData.Materials.Add(primitive.Material);
            }

            if (!_renderData.Graph[primitive.Shader][primitive.Material].ContainsKey(transform))
            {
                _renderData.Graph[primitive.Shader][primitive.Material].Add(transform, new List<VertexPrimitiveAsset>());
                _renderData.Transforms.Add(transform);
            }

            _renderData.Graph[primitive.Shader][primitive.Material][transform].Add(primitive.Primitive);
            _renderData.Primitves.Add(primitive.Primitive);
        }
    }
}
