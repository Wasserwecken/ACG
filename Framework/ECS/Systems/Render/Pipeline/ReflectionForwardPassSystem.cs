using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Materials;
using Framework.Assets.Shader;
using Framework.Assets.Shader.Block;
using Framework.ECS.Components.Light;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Scene;
using Framework.ECS.Components.Transform;
using Framework.ECS.Systems.Render.OpenGL;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace Framework.ECS.Systems.Render.Pipeline
{
    [With(typeof(TransformComponent))]
    [With(typeof(ReflectionProbeComponent))]
    public class ReflectionForwardPassSystem : AEntitySetSystem<bool>
    {
        private readonly ShaderViewSpaceBlock _viewBlock;
        private readonly Entity _worldComponents;
        private readonly EntitySet _renderCandidates;
        private readonly Dictionary<ShaderProgramAsset, Dictionary<MaterialAsset, List<PrimitiveComponent>>> _graph;

        /// <summary>
        /// 
        /// </summary>
        public ReflectionForwardPassSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
            _renderCandidates = World.GetEntities()
                .With<TransformComponent>()
                .With<PrimitiveComponent>()
                .AsSet();

            _viewBlock = new ShaderViewSpaceBlock();
            _graph = new Dictionary<ShaderProgramAsset, Dictionary<MaterialAsset, List<PrimitiveComponent>>>();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void PreUpdate(bool state)
        {
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
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, in Entity entity)
        {

            ref var transform = ref entity.Get<TransformComponent>();
            ref var buffer = ref _worldComponents.Get<ReflectionBufferComponent>();
            ref var probeConfig = ref entity.Get<ReflectionProbeComponent>();

            if (!probeConfig.HasChanged) return;
            probeConfig.HasChanged = false;
            
            Renderer.Use(buffer.DeferredLightBuffer);

            var cubeOrientations = Helper.CreateCubeOrientations(transform.Position);
            for (int c = 0; c < cubeOrientations.Length; c++)
            {
                // VIEWPORT PREPERATION
                var viewPort = buffer.ReflectionBlock.Probes[probeConfig.InfoID].Area.Xyz * new Vector3(
                    buffer.DeferredGBuffer.Width,
                    buffer.DeferredGBuffer.Height,
                    buffer.DeferredGBuffer.Width);
                var width = (int)viewPort.Z / 3;
                var height = (int)viewPort.Z / 2;
                var x = (int)viewPort.X + (c % 3) * width;
                var y = (int)viewPort.Y + (c < 3 ? 0 : height);
                GL.Viewport(x, y, width, height);

                // VIEW SPACE SETUP
                var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90f), 1f, probeConfig.NearClipping, probeConfig.FarClipping);
                _viewBlock.WorldToView = cubeOrientations[c];
                _viewBlock.WorldToProjection = cubeOrientations[c] * projection;
                _viewBlock.WorldToViewRotation = cubeOrientations[c].ClearScale().ClearTranslation();
                _viewBlock.WorldToProjectionRotation = cubeOrientations[c].ClearScale().ClearTranslation() * projection;
                _viewBlock.ViewPosition = new Vector4(transform.Position, 1);
                _viewBlock.ViewDirection = new Vector4(cubeOrientations[c].Row2);
                _viewBlock.Resolution = new Vector2(probeConfig.Resolution, probeConfig.Resolution);
                GPUSync.Push(_viewBlock);

                // RENDER SCENE
                foreach (var shaderRelation in _graph)
                {
                    Renderer.Use(shaderRelation.Key);
                    Renderer.Use(_viewBlock, shaderRelation.Key);
                    foreach (var materialRelation in shaderRelation.Value)
                    {
                        Renderer.Use(materialRelation.Key, shaderRelation.Key);
                        foreach (var primitive in materialRelation.Value)
                        {
                            Renderer.Use(primitive.PrimitiveSpaceBlock, shaderRelation.Key);
                            Renderer.Draw(primitive.Verticies);
                        }
                    }
                }

                Renderer.Use(Defaults.Shader.Program.Skybox);
                Renderer.Use(_viewBlock, Defaults.Shader.Program.Skybox);
                Renderer.Use(Defaults.Material.Skybox, Defaults.Shader.Program.Skybox);
                Renderer.Draw(Defaults.Vertex.Mesh.Cube[0]);
            }
        }
    }
}
