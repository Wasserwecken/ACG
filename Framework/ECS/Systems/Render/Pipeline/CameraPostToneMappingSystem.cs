using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Materials;
using Framework.ECS.Components.PostProcessing;
using Framework.ECS.Components.Render;
using Framework.ECS.Systems.Render.OpenGL;
using OpenTK.Graphics.OpenGL;

namespace Framework.ECS.Systems.Render.Pipeline
{
    [With(typeof(TonemappingComponent))]
    [With(typeof(PerspectiveCameraComponent))]
    public class CameraPostToneMappingSystem : AEntitySetSystem<bool>
    {
        private readonly Entity _worldComponents;
        private readonly MaterialAsset _postMaterial;

        /// <summary>
        /// 
        /// </summary>
        public CameraPostToneMappingSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
            _postMaterial = new MaterialAsset("PostTonemaping") { DepthTest = DepthFunction.Always };
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, in Entity entity)
        {
            var config = entity.Get<TonemappingComponent>();
            var camera = entity.Get<PerspectiveCameraComponent>();

            _postMaterial.SetUniform("BufferMap", camera.DeferredLightBuffer.Textures[0]);
            _postMaterial.SetUniform("Exposure", config.Exposure);

            Renderer.Use(camera.DeferredLightBuffer);
            Renderer.Use(Defaults.Shader.Program.PostTonemapping);
            Renderer.Use(_postMaterial, Defaults.Shader.Program.PostTonemapping);

            Renderer.Draw(Defaults.Vertex.Mesh.Plane[0]);
        }
    }
}
