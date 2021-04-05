using Framework.Assets.Materials;
using Framework.Assets.Verticies;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Scene;
using Framework.ECS.Components.Transform;
using System.Collections.Generic;
using System.Linq;

namespace Framework.ECS.Systems.Hierarchy
{
    public class RenderHierarchySystem : ISystem
    {
        /// <summary>
        /// 
        /// </summary>
        public void Run(IEnumerable<Entity> entities, IEnumerable<IComponent> sceneComponents)
        {
            var renderDataComponent = sceneComponents.First(f => f is RenderDataComponent) as RenderDataComponent;
            renderDataComponent.Graph.Clear();
            renderDataComponent.Shaders.Clear();
            renderDataComponent.Materials.Clear();
            renderDataComponent.Transforms.Clear();
            renderDataComponent.Primitves.Clear();

            var meshes = entities.Where(f => f.Components.Has<MeshComponent>());
            foreach (var meshEntity in meshes)
            {
                var mesh = meshEntity.Components.Get<MeshComponent>();
                var transform = meshEntity.Components.Get<TransformComponent>();

                for (int i = 0; i < mesh.Mesh.Primitives.Count; i++)
                {
                    var shader = mesh.Shaders[mesh.Shaders.Count > i ? i : 0];
                    var material = mesh.Materials[mesh.Materials.Count > i ? i : 0];
                    var primitive = mesh.Mesh.Primitives[i];

                    if (!renderDataComponent.Graph.ContainsKey(shader))
                    {
                        renderDataComponent.Graph.Add(shader, new Dictionary<MaterialAsset, Dictionary<TransformComponent, List<VertexPrimitiveAsset>>>());
                        renderDataComponent.Shaders.Add(shader);
                    }

                    if (!renderDataComponent.Graph[shader].ContainsKey(material))
                    {
                        renderDataComponent.Graph[shader].Add(material, new Dictionary<TransformComponent, List<VertexPrimitiveAsset>>());
                        renderDataComponent.Materials.Add(material);
                    }

                    if (!renderDataComponent.Graph[shader][material].ContainsKey(transform))
                    {
                        renderDataComponent.Graph[shader][material].Add(transform, new List<VertexPrimitiveAsset>());
                        renderDataComponent.Transforms.Add(transform);
                    }

                    renderDataComponent.Graph[shader][material][transform].Add(primitive);
                    renderDataComponent.Primitves.Add(primitive);
                }
            }
        }
    }
}
