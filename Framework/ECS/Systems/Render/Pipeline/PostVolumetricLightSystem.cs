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
    [With(typeof(PostVolumetricLightComponent))]
    [With(typeof(PerspectiveCameraComponent))]
    public class PostVolumetricLightSystem : AEntitySetSystem<bool>
    {

        private readonly Entity _worldComponents;
        private readonly MaterialAsset _postMaterial;

        /// <summary>
        /// 
        /// </summary>
        public PostVolumetricLightSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
            _postMaterial = new MaterialAsset("VolumetricSettings") { DepthTest = DepthFunction.Always };
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, in Entity entity)
        {
            ref var config = ref entity.Get<PostVolumetricLightComponent>();
            ref var camera = ref entity.Get<PerspectiveCameraComponent>();

            if (config.SamplingBuffer == null)
                config.SamplingBuffer = CreateBuffer();

            if (config.SamplingBuffer.Width != camera.DeferredLightBuffer.Width || config.SamplingBuffer.Height != camera.DeferredLightBuffer.Height)
            {
                config.SamplingBuffer.Handle = 0;
                config.SamplingBuffer.Width = camera.DeferredLightBuffer.Width;
                config.SamplingBuffer.Height = camera.DeferredLightBuffer.Height;
            }

            if (config.ResultBuffer == null)
                config.ResultBuffer = CreateBuffer();

            if (config.ResultBuffer.Width != camera.DeferredLightBuffer.Width || config.ResultBuffer.Height != camera.DeferredLightBuffer.Height)
            {
                config.ResultBuffer.Handle = 0;
                config.ResultBuffer.Width = camera.DeferredLightBuffer.Width;
                config.ResultBuffer.Height = camera.DeferredLightBuffer.Height;
            }


            // TRACE Volumetrics
            _postMaterial.SetUniform("ViewPosition", camera.ShaderViewSpace.ViewPosition);
            _postMaterial.SetUniform("Projection", camera.ShaderViewSpace.WorldToProjection);
            _postMaterial.SetUniform("ProjectionInverse", camera.ShaderViewSpace.Projection.Inverted());
            _postMaterial.SetUniform("ViewInverse", camera.ShaderViewSpace.WorldToView.Inverted());
            foreach (var texture in camera.DeferredGBuffer.Textures)
                _postMaterial.SetUniform(texture.Name, texture);

            Renderer.Use(config.SamplingBuffer);
            Renderer.Use(Defaults.Shader.Program.PostVolumetricSample);
            Renderer.Use(_postMaterial, Defaults.Shader.Program.PostVolumetricSample);

            GL.Viewport(0, 0, config.SamplingBuffer.Width, config.SamplingBuffer.Height);
            GL.ClearColor(config.SamplingBuffer.ClearColor);
            GL.Clear(config.SamplingBuffer.ClearMask);

            Renderer.Draw(Defaults.Vertex.Mesh.Plane[0]);


            // COPY RESULT
            Renderer.Blit(config.SamplingBuffer, camera.DeferredLightBuffer, ClearBufferMask.ColorBufferBit);
        }


        /// <summary>
        /// 
        /// </summary>
        private FramebufferAsset CreateBuffer()
        {
            return new FramebufferAsset("VolumetricBuffer")
            {
                DrawTargets = new DrawBuffersEnum[] { DrawBuffersEnum.ColorAttachment0 },
                Textures = new List<TextureRenderAsset>()
                {
                    new TextureRenderAsset("Illumination")
                    {
                        Attachment = FramebufferAttachment.ColorAttachment0,
                        InternalFormat = PixelInternalFormat.Rgba16f,
                        Format = PixelFormat.Rgb,
                        PixelType = PixelType.Float,
                    }
                }
            };
        }
    }
}
