using Framework.Assets.Materials;
using Framework.Assets.Verticies;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Scene;
using Framework.ECS.Components.Transform;
using System.Collections.Generic;
using System.Linq;

namespace Framework.ECS.Systems
{
    public class RenderHierarchySystem : ISystem
    {
        /// <summary>
        /// 
        /// </summary>
        public void Update(IEnumerable<Entity> entities, IEnumerable<IComponent> sceneComponents)
        {
            var dataComponent = sceneComponents.First(f => f is RenderDataComponent) as RenderDataComponent;
            dataComponent.Graph.Clear();
            dataComponent.Entities.Clear();
            dataComponent.Shaders.Clear();
            dataComponent.Materials.Clear();
            dataComponent.Transforms.Clear();
            dataComponent.Primitves.Clear();

            var meshes = entities.Where(f => f.HasAnyComponents(typeof(MeshComponent)));
            foreach (var meshEntity in meshes)
            {
                var mesh = meshEntity.GetComponent<MeshComponent>();
                var transform = meshEntity.GetComponent<TransformComponent>();
                dataComponent.Entities.Add(meshEntity);

                for (int i = 0; i < mesh.Mesh.Primitives.Count; i++)
                {
                    var shader = mesh.Shaders[mesh.Shaders.Count > i ? i : 0];
                    var material = mesh.Materials[mesh.Materials.Count > i ? i : 0];
                    var primitive = mesh.Mesh.Primitives[i];

                    if (!dataComponent.Graph.ContainsKey(shader))
                    {
                        dataComponent.Graph.Add(shader, new Dictionary<MaterialAsset, Dictionary<TransformComponent, List<VertexPrimitiveAsset>>>());
                        dataComponent.Shaders.Add(shader);
                    }

                    if (!dataComponent.Graph[shader].ContainsKey(material))
                    {
                        dataComponent.Graph[shader].Add(material, new Dictionary<TransformComponent, List<VertexPrimitiveAsset>>());
                        dataComponent.Materials.Add(material);
                    }

                    if (!dataComponent.Graph[shader][material].ContainsKey(transform))
                    {
                        dataComponent.Graph[shader][material].Add(transform, new List<VertexPrimitiveAsset>());
                        dataComponent.Transforms.Add(transform);
                    }

                    dataComponent.Graph[shader][material][transform].Add(primitive);
                    dataComponent.Primitves.Add(primitive);
                }
            }
        }
    }
}
