using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Materials;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Scene;
using Framework.ECS.Components.Transform;
using Framework.ECS.Systems.Render.OpenGL;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Framework.ECS.Systems.Render.Pipeline
{
    [With(typeof(TransformComponent))]
    [With(typeof(PerspectiveCameraComponent))]
    public class CameraPublishSystem : AEntitySetSystem<bool>
    {
        private readonly Entity _worldComponents;
        private readonly MaterialAsset _bufferMaterial;

        /// <summary>
        /// 
        /// </summary>
        public CameraPublishSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
            _bufferMaterial = new MaterialAsset("BufferDisplay") { DepthTest = DepthFunction.Always };
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, in Entity entity)
        {
            var aspect = _worldComponents.Get<AspectRatioComponent>();
            var camera = entity.Get<PerspectiveCameraComponent>();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, aspect.Width, aspect.Height);
            GL.ClearColor(Color4.Pink);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            _bufferMaterial.SetUniform("BufferMap", camera.DeferredLightBuffer.Textures[0]);

            Renderer.Use(Defaults.Shader.Program.FrameBuffer);
            Renderer.Use(_bufferMaterial, Defaults.Shader.Program.FrameBuffer);
            Renderer.Draw(Defaults.Vertex.Mesh.Plane[0]);
        }
    }
}
