using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Materials;
using Framework.Assets.Shader;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Transform;
using Framework.ECS.Systems.Render.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.ECS.Systems.Render.Pipeline
{
    [With(typeof(TransformComponent))]
    [With(typeof(PerspectiveCameraComponent))]
    public class CameraForwardPassSystem : AEntitySetSystem<bool>
    {
        private readonly Entity _worldComponents;
        private readonly EntitySet _renderCandidates;
        private readonly Dictionary<ShaderProgramAsset, Dictionary<MaterialAsset, List<PrimitiveComponent>>> _graph;

        /// <summary>
        /// 
        /// </summary>
        public CameraForwardPassSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
            _renderCandidates = World.GetEntities()
                .With<TransformComponent>()
                .With<PrimitiveComponent>()
                .AsSet();

            _graph = new Dictionary<ShaderProgramAsset, Dictionary<MaterialAsset, List<PrimitiveComponent>>>();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, in Entity entity)
        {
            var camera = entity.Get<PerspectiveCameraComponent>();

            _graph.Clear();
            foreach (ref readonly var candidate in _renderCandidates.GetEntities())
            {
                var primitive = candidate.Get<PrimitiveComponent>();
                if (primitive.Shader != Defaults.Shader.Program.MeshLitDeferredLight)
                {
                    if (!_graph.ContainsKey(primitive.Shader))
                        _graph.Add(primitive.Shader, new Dictionary<MaterialAsset, List<PrimitiveComponent>>());

                    if (!_graph[primitive.Shader].ContainsKey(primitive.Material))
                        _graph[primitive.Shader].Add(primitive.Material, new List<PrimitiveComponent>());

                    _graph[primitive.Shader][primitive.Material].Add(primitive);
                }
            }

            Renderer.Use(camera.DeferredLightBuffer);
            foreach(var shaderRelation in _graph)
            {
                Renderer.Use(shaderRelation.Key);
                Renderer.Use(camera.ShaderViewSpace, shaderRelation.Key);
                foreach(var materialRelation in shaderRelation.Value)
                {
                    Renderer.Use(materialRelation.Key, shaderRelation.Key);
                    foreach(var primitive in materialRelation.Value)
                    {
                        Renderer.Use(primitive.PrimitiveSpaceBlock, shaderRelation.Key);
                        Renderer.Draw(primitive.Verticies);
                    }
                }
            }

            Renderer.Use(Defaults.Shader.Program.Skybox);
            Renderer.Use(camera.ShaderViewSpace, Defaults.Shader.Program.Skybox);
            Renderer.Use(Defaults.Material.Skybox, Defaults.Shader.Program.Skybox);
            Renderer.Draw(Defaults.Vertex.Mesh.Cube[0]);
        }
    }
}
