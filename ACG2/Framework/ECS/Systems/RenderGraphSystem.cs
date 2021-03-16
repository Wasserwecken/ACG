using Framework.Assets.Materials;
using Framework.Assets.Verticies;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Scene;
using Framework.ECS.Components.Transform;
using System.Collections.Generic;
using System.Linq;

namespace Framework.ECS.Systems
{
    public class RenderGraphSystem : ISystem
    {
        /// <summary>
        /// 
        /// </summary>
        public void Update(IEnumerable<Entity> entities, IEnumerable<IComponent> sceneComponents)
        {
            var graphComponent = sceneComponents.First(f => f is RenderGraphComponent) as RenderGraphComponent;
            graphComponent.Graph.Clear();
            graphComponent.Entities.Clear();
            graphComponent.Shaders.Clear();
            graphComponent.Materials.Clear();
            graphComponent.Transforms.Clear();
            graphComponent.Primitves.Clear();

            var meshes = entities.Where(f => f.HasAnyComponents(typeof(MeshComponent)));
            foreach (var meshEntity in meshes)
            {
                var mesh = meshEntity.GetComponent<MeshComponent>();
                var transform = meshEntity.GetComponent<TransformComponent>();
                graphComponent.Entities.Add(meshEntity);

                for (int i = 0; i < mesh.Mesh.Primitives.Count; i++)
                {
                    var shader = mesh.Shaders[mesh.Shaders.Count > i ? i : 0];
                    var material = mesh.Materials[mesh.Materials.Count > i ? i : 0];
                    var primitive = mesh.Mesh.Primitives[i];

                    if (primitive.Handle <= 0) primitive.PushToGPU();

                    if (!graphComponent.Graph.ContainsKey(shader))
                    {
                        graphComponent.Graph.Add(shader, new Dictionary<MaterialAsset, Dictionary<TransformComponent, List<VertexPrimitiveAsset>>>());
                        graphComponent.Shaders.Add(shader);
                    }

                    if (!graphComponent.Graph[shader].ContainsKey(material))
                    {
                        graphComponent.Graph[shader].Add(material, new Dictionary<TransformComponent, List<VertexPrimitiveAsset>>());
                        graphComponent.Materials.Add(material);
                    }

                    if (!graphComponent.Graph[shader][material].ContainsKey(transform))
                    {
                        graphComponent.Graph[shader][material].Add(transform, new List<VertexPrimitiveAsset>());
                        graphComponent.Transforms.Add(transform);
                    }

                    graphComponent.Graph[shader][material][transform].Add(primitive);
                    graphComponent.Primitves.Add(primitive);
                }
            }
        }
    }
}
