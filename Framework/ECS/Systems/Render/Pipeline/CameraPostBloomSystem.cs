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
    [With(typeof(PostBloomComponent))]
    [With(typeof(PerspectiveCameraComponent))]
    public class CameraPostBloomSystem : AEntitySetSystem<bool>
    {
        private readonly Entity _worldComponents;
        private readonly MaterialAsset _postMaterial;

        /// <summary>
        /// 
        /// </summary>
        public CameraPostBloomSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
            _postMaterial = new MaterialAsset("PostBloom") { IsWritingDepth = false, DepthTest = DepthFunction.Always };
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, in Entity entity)
        {
            ref var config = ref entity.Get<PostBloomComponent>();
            ref var camera = ref entity.Get<PerspectiveCameraComponent>();


            if (config.BufferA == null)
                config.BufferA = CreateBloomBuffer();
            
            if (config.BufferB == null)
                config.BufferB = CreateBloomBuffer();

            if (config.BufferA.Width != camera.DeferredLightBuffer.Width || config.BufferA.Height != camera.DeferredLightBuffer.Height)
            {
                config.BufferA.Handle = 0;
                config.BufferA.Width = camera.DeferredLightBuffer.Width;
                config.BufferA.Height = camera.DeferredLightBuffer.Height;
            }
            
            if (config.BufferB.Width != camera.DeferredLightBuffer.Width || config.BufferB.Height != camera.DeferredLightBuffer.Height)
            {
                config.BufferB.Handle = 0;
                config.BufferB.Width = camera.DeferredLightBuffer.Width;
                config.BufferB.Height = camera.DeferredLightBuffer.Height;
            }


            // SELECT FRAGMENTS
            _postMaterial.SetUniform("BufferMap", camera.DeferredLightBuffer.Textures[0]);
            _postMaterial.SetUniform("ThresholdStart", config.ThresholdStart);
            _postMaterial.SetUniform("ThresholdEnd", config.ThresholdEnd);
            _postMaterial.SetUniform("Intensity", config.Intensity);

            Renderer.Use(config.BufferA);
            Renderer.Use(Defaults.Shader.Program.PostBloomSelect);
            Renderer.Use(_postMaterial, Defaults.Shader.Program.PostBloomSelect);
            Renderer.Draw(Defaults.Vertex.Mesh.Plane[0]);

            // BLUR FRAGMENTS
            Renderer.Use(Defaults.Shader.Program.PostGaussianBlur);
            for (int i = 0; i < config.Samples; i++)
            {
                _postMaterial.SetUniform("BufferMap", config.BufferA.Textures[0]);
                _postMaterial.SetUniform("Horizontal", 0f);

                Renderer.Use(config.BufferB);
                Renderer.Use(_postMaterial, Defaults.Shader.Program.PostGaussianBlur);
                Renderer.Draw(Defaults.Vertex.Mesh.Plane[0]);


                Renderer.Use(config.BufferA);
                _postMaterial.SetUniform("BufferMap", config.BufferB.Textures[0]);
                _postMaterial.SetUniform("Horizontal", 1f);

                Renderer.Use(_postMaterial, Defaults.Shader.Program.PostGaussianBlur);
                Renderer.Draw(Defaults.Vertex.Mesh.Plane[0]);
            }

            // MERGE RESULT
            _postMaterial.SetUniform("BloomMap", config.BufferA.Textures[0]);
            _postMaterial.SetUniform("BufferMap", camera.DeferredLightBuffer.Textures[0]);

            Renderer.Use(config.BufferB);
            Renderer.Use(Defaults.Shader.Program.PostBloomMerge);
            Renderer.Use(_postMaterial, Defaults.Shader.Program.PostBloomMerge);
            Renderer.Draw(Defaults.Vertex.Mesh.Plane[0]);

            // COPY RESULT
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, config.BufferB.Handle);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, camera.DeferredLightBuffer.Handle);
            GL.BlitFramebuffer(
                0, 0, config.BufferB.Width, config.BufferB.Height,
                0, 0, camera.DeferredLightBuffer.Width, camera.DeferredLightBuffer.Height,
                ClearBufferMask.ColorBufferBit,
                BlitFramebufferFilter.Nearest
            );
        }

        /// <summary>
        /// 
        /// </summary>
        private FramebufferAsset CreateBloomBuffer()
        {
            return new FramebufferAsset("BloomBuffer")
            {
                DrawTargets = new DrawBuffersEnum[] { DrawBuffersEnum.ColorAttachment0 },
                Textures = new List<TextureRenderAsset>()
                {
                    new TextureRenderAsset("Bloom")
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
