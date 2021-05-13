using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Materials;
using Framework.Assets.Shader;
using Framework.Assets.Shader.Block;
using Framework.Assets.Textures;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Scene;
using Framework.ECS.Components.Transform;
using Framework.ECS.Systems.Render.OpenGL;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace Framework.ECS.Systems.Render
{
    [With(typeof(TransformComponent))]
    [With(typeof(PerspectiveCameraComponent))]
    public class ForwardPassSystemOLD : AEntitySetSystem<bool>
    {
        private readonly Entity _worldComponents;
        private EntitySet _graphSet;
        private readonly Dictionary<ShaderProgramAsset,
           Dictionary<MaterialAsset,
                List<PrimitiveComponent>>> _graph;

        /// <summary>
        /// 
        /// </summary>
        public ForwardPassSystemOLD(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
            _graphSet = World.GetEntities().With<TransformComponent>().With<PrimitiveComponent>().AsSet();
            _graph = new Dictionary<ShaderProgramAsset, Dictionary<MaterialAsset, List<PrimitiveComponent>>>();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void PreUpdate(bool state)
        {
            _graph.Clear();

            foreach (var entity in _graphSet.GetEntities())
            {
                var primitive = entity.Get<PrimitiveComponent>();

                var shader = primitive.Shader;
                var material = primitive.Material;

                if (!_graph.ContainsKey(shader))
                    _graph.Add(shader, new Dictionary<MaterialAsset, List<PrimitiveComponent>>());

                if (!_graph[shader].ContainsKey(material))
                    _graph[shader].Add(material, new List<PrimitiveComponent>());

                _graph[shader][material].Add(primitive);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, in Entity entity)
        {
            var cameraData = entity.Get<PerspectiveCameraComponent>();
            var cameraTransform = entity.Get<TransformComponent>();
            var aspectRatio = _worldComponents.Get<AspectRatioComponent>();


            TextureBaseAsset skyboxTexture = Defaults.Texture.SkyboxCoast;
            var projectionSpace = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(cameraData.FieldOfView),
                aspectRatio.Ratio,
                cameraData.NearClipping,
                cameraData.FarClipping
            );

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, aspectRatio.Width, aspectRatio.Height);
            GL.ClearColor(cameraData.ClearColor.X, cameraData.ClearColor.Y, cameraData.ClearColor.Z, cameraData.ClearColor.W);
            GL.Clear(cameraData.ClearMask);

            if (cameraData.ShaderViewSpace == null)
                cameraData.ShaderViewSpace = new ShaderViewSpaceBlock();

            cameraData.ShaderViewSpace.WorldToView = cameraTransform.WorldSpaceInverse;
            cameraData.ShaderViewSpace.WorldToProjection = cameraTransform.WorldSpaceInverse * projectionSpace;
            cameraData.ShaderViewSpace.WorldToViewRotation = cameraTransform.WorldSpaceInverse.ClearScale().ClearTranslation();
            cameraData.ShaderViewSpace.WorldToProjectionRotation = cameraTransform.WorldSpaceInverse.ClearScale().ClearTranslation() * projectionSpace;
            cameraData.ShaderViewSpace.ViewPosition = new Vector4(cameraTransform.Position, 1);
            cameraData.ShaderViewSpace.ViewDirection = new Vector4(cameraTransform.Forward, 0);
            cameraData.ShaderViewSpace.Resolution = new Vector2(aspectRatio.Width, aspectRatio.Height);

            GPUSync.Push(cameraData.ShaderViewSpace);

            foreach (var shaderRelation in _graph)
            {
                Renderer.UseShader(shaderRelation.Key);
                Renderer.UseShaderBlock(cameraData.ShaderViewSpace, shaderRelation.Key);

                foreach (var materialRelation in shaderRelation.Value)
                {
                    materialRelation.Key.SetUniform(Definitions.Shader.Uniform.ReflectionMap, skyboxTexture);
                    Renderer.UseMaterial(materialRelation.Key, shaderRelation.Key);

                    foreach (var primitive in materialRelation.Value)
                    {
                        Renderer.UseShaderBlock(primitive.ShaderSpaceBlock, shaderRelation.Key);
                        Renderer.Draw(primitive.Verticies);
                    }
                }
            }
        }
    }
}
