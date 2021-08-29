using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Framebuffer;
using Framework.Assets.Materials;
using Framework.Assets.Textures;
using Framework.ECS.Components.PostProcessing;
using Framework.ECS.Components.Render;
using Framework.ECS.Systems.Render.OpenGL;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace Framework.ECS.Systems.Render.Pipeline
{
    [With(typeof(PostVolumetricLightComponent))]
    [With(typeof(PerspectiveCameraComponent))]
    public class PostVolumetricLightSystem : AEntitySetSystem<bool>
    {

        private readonly Entity _worldComponents;
        private readonly MaterialAsset _samplingMaterial;
        private readonly MaterialAsset _blurMaterial;
        private readonly MaterialAsset _addMaterial;

        /// <summary>
        /// 
        /// </summary>
        public PostVolumetricLightSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
            _samplingMaterial = new MaterialAsset("VolumetricSampling") { DepthTest = DepthFunction.Always };
            _blurMaterial = new MaterialAsset("BilateralBlur") { DepthTest = DepthFunction.Always };
            _addMaterial = new MaterialAsset("Add") { DepthTest = DepthFunction.Always };
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, in Entity entity)
        {
            ref var config = ref entity.Get<PostVolumetricLightComponent>();
            ref var camera = ref entity.Get<PerspectiveCameraComponent>();

            var samplingWidth = (int)(camera.DeferredLightBuffer.Width * config.SamplingBufferScale);
            var samplingHeight = (int)(camera.DeferredLightBuffer.Height * config.SamplingBufferScale);

            if (config.SamplingBuffer == null)
                config.SamplingBuffer = CreateBuffer();

            if (config.SamplingBuffer.Width != samplingWidth || config.SamplingBuffer.Height != samplingHeight)
            {
                config.SamplingBuffer.Handle = 0;
                config.SamplingBuffer.Width = samplingWidth;
                config.SamplingBuffer.Height = samplingHeight;
            }

            if (config.BlurBuffer == null)
                config.BlurBuffer = CreateBuffer();

            if (config.BlurBuffer.Width != samplingWidth || config.BlurBuffer.Height != samplingHeight)
            {
                config.BlurBuffer.Handle = 0;
                config.BlurBuffer.Width = samplingWidth;
                config.BlurBuffer.Height = samplingHeight;
            }

            if (config.AddBuffer == null)
                config.AddBuffer = CreateBuffer();

            if (config.AddBuffer.Width != camera.DeferredLightBuffer.Width || config.AddBuffer.Height != camera.DeferredLightBuffer.Height)
            {
                config.AddBuffer.Handle = 0;
                config.AddBuffer.Width = camera.DeferredLightBuffer.Width;
                config.AddBuffer.Height = camera.DeferredLightBuffer.Height;
            }

            // TRACE Volumetrics
            _samplingMaterial.SetUniform("ViewPosition", camera.ShaderViewSpace.ViewPosition);
            _samplingMaterial.SetUniform("ClusterSize", config.SamplingClusterSize);
            _samplingMaterial.SetUniform("MarchStepMaxSize", config.SamplingMarchStepMaxSize);
            _samplingMaterial.SetUniform("MarchStepMaxCount", config.SamplingMarchStepMaxCount);
            _samplingMaterial.SetUniform("VolumeColor", config.VolumeColor);
            _samplingMaterial.SetUniform("VolumeDensity", config.VolumeDensity);
            _samplingMaterial.SetUniform("VolumeScattering", 1f - config.VolumeScattering);

            foreach (var texture in camera.DeferredGBuffer.Textures)
                _samplingMaterial.SetUniform(texture.Name, texture);

            Renderer.Use(config.SamplingBuffer);
            Renderer.Use(Defaults.Shader.Program.PostVolumetricSample);
            Renderer.Use(_samplingMaterial, Defaults.Shader.Program.PostVolumetricSample);

            GL.Viewport(0, 0, config.SamplingBuffer.Width, config.SamplingBuffer.Height);
            GL.ClearColor(config.SamplingBuffer.ClearColor);
            GL.Clear(config.SamplingBuffer.ClearMask);

            Renderer.Draw(Defaults.Vertex.Mesh.Plane[0]);

            // BLUR 1
            _blurMaterial.SetUniform("BufferMap", config.SamplingBuffer.Textures[0]);
            _blurMaterial.SetUniform("Horizontal", 0f);
            _blurMaterial.SetUniform("Clipping", new Vector4(camera.NearClipping, camera.FarClipping, 0f, 0f));
            foreach (var texture in camera.DeferredGBuffer.Textures)
                _blurMaterial.SetUniform(texture.Name, texture);

            Renderer.Use(config.BlurBuffer);
            Renderer.Use(Defaults.Shader.Program.PostBilaterlBlur);
            Renderer.Use(_blurMaterial, Defaults.Shader.Program.PostBilaterlBlur);

            GL.Viewport(0, 0, config.BlurBuffer.Width, config.BlurBuffer.Height);
            GL.ClearColor(config.BlurBuffer.ClearColor);
            GL.Clear(config.BlurBuffer.ClearMask);

            Renderer.Draw(Defaults.Vertex.Mesh.Plane[0]);

            // BLUR 2
            _blurMaterial.SetUniform("BufferMap", config.BlurBuffer.Textures[0]);
            _blurMaterial.SetUniform("Horizontal", 1f);
            foreach (var texture in camera.DeferredGBuffer.Textures)
                _blurMaterial.SetUniform(texture.Name, texture);

            Renderer.Use(config.SamplingBuffer);
            Renderer.Use(Defaults.Shader.Program.PostBilaterlBlur);
            Renderer.Use(_blurMaterial, Defaults.Shader.Program.PostBilaterlBlur);

            GL.Viewport(0, 0, config.SamplingBuffer.Width, config.SamplingBuffer.Height);
            GL.ClearColor(config.SamplingBuffer.ClearColor);
            GL.Clear(config.SamplingBuffer.ClearMask);

            Renderer.Draw(Defaults.Vertex.Mesh.Plane[0]);


            // ADD
            _addMaterial.SetUniform("TextureA", config.SamplingBuffer.Textures[0]);
            _addMaterial.SetUniform("TextureB", camera.DeferredLightBuffer.Textures[0]);

            Renderer.Use(config.AddBuffer);
            Renderer.Use(Defaults.Shader.Program.PostAdd);
            Renderer.Use(_addMaterial, Defaults.Shader.Program.PostAdd);

            GL.Viewport(0, 0, config.AddBuffer.Width, config.AddBuffer.Height);
            GL.ClearColor(config.AddBuffer.ClearColor);
            GL.Clear(config.AddBuffer.ClearMask);

            Renderer.Draw(Defaults.Vertex.Mesh.Plane[0]);


            // COPY RESULT
            Renderer.Blit(config.AddBuffer, camera.DeferredLightBuffer, ClearBufferMask.ColorBufferBit);
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
