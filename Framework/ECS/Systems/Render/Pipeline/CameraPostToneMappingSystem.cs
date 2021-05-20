using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Framebuffer;
using Framework.Assets.Materials;
using Framework.Assets.Textures;
using Framework.ECS.Components.PostProcessing;
using Framework.ECS.Components.Render;
using Framework.ECS.Systems.Render.OpenGL;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace Framework.ECS.Systems.Render.Pipeline
{
    [With(typeof(PostTonemappingComponent))]
    [With(typeof(PerspectiveCameraComponent))]
    public class CameraPostTonemappingSystem : AEntitySetSystem<bool>
    {
        private readonly Entity _worldComponents;
        private readonly MaterialAsset _postMaterial;

        /// <summary>
        /// 
        /// </summary>
        public CameraPostTonemappingSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
            _postMaterial = new MaterialAsset("PostTonemaping") { DepthTest = DepthFunction.Always };
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, in Entity entity)
        {
            ref var config = ref entity.Get<PostTonemappingComponent>();
            ref var camera = ref entity.Get<PerspectiveCameraComponent>();

            if (config.Buffer == null)
                config.Buffer = CreateTonemappingBuffer();

            if (config.Buffer.Width != camera.DeferredLightBuffer.Width || config.Buffer.Height != camera.DeferredLightBuffer.Height)
            {
                config.Buffer.Handle = 0;
                config.Buffer.Width = camera.DeferredLightBuffer.Width;
                config.Buffer.Height = camera.DeferredLightBuffer.Height;
            }


            _postMaterial.SetUniform("BufferMap", camera.DeferredLightBuffer.Textures[0]);
            _postMaterial.SetUniform("Exposure", config.Exposure);

            GL.Viewport(0, 0, camera.DeferredLightBuffer.Width, camera.DeferredLightBuffer.Height);

            // APPLY MAPPING
            Renderer.Use(config.Buffer);
            Renderer.Use(Defaults.Shader.Program.PostTonemapping);
            Renderer.Use(_postMaterial, Defaults.Shader.Program.PostTonemapping);
            Renderer.Draw(Defaults.Vertex.Mesh.Plane[0]);

            // COPY RESULT
            Renderer.Blit(config.Buffer, camera.DeferredLightBuffer, ClearBufferMask.ColorBufferBit);
        }

        /// <summary>
        /// 
        /// </summary>
        private FramebufferAsset CreateTonemappingBuffer()
        {
            return new FramebufferAsset("TonemappingBuffer")
            {
                DrawTargets = new DrawBuffersEnum[] { DrawBuffersEnum.ColorAttachment0 },
                Textures = new List<TextureRenderAsset>()
                {
                    new TextureRenderAsset("Tonemapping")
                    {
                        Attachment = FramebufferAttachment.ColorAttachment0,
                        InternalFormat = PixelInternalFormat.Rgba16f,
                        Format = PixelFormat.Rgba,
                        PixelType = PixelType.Float,
                    }
                }
            };
        }
    }
}
