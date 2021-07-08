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
    [With(typeof(PerspectiveCameraComponent))]
    [With(typeof(PostGlobalIllumination))]
    public class CameraPostGlobalIllumination : AEntitySetSystem<bool>
    {
        private readonly Entity _worldComponents;
        private readonly MaterialAsset _postTraceMaterial;
        private readonly MaterialAsset _postSampleMaterial;
        private readonly MaterialAsset _postBlurMaterial;

        private int _sampleBufferIndexX;
        private int _sampleBufferIndexY;

        /// <summary>
        /// 
        /// </summary>
        public CameraPostGlobalIllumination(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
            _postTraceMaterial = new MaterialAsset("PostIllumination") { IsWritingDepth = false, DepthTest = DepthFunction.Always };
            _postSampleMaterial = new MaterialAsset("PostIllumination") { IsWritingDepth = false, DepthTest = DepthFunction.Always };
            _postBlurMaterial = new MaterialAsset("PostIllumination") { IsWritingDepth = false, DepthTest = DepthFunction.Always };

            _sampleBufferIndexX = 0;
            _sampleBufferIndexY = 0;
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, in Entity entity)
        {
            ref var config = ref entity.Get<PostGlobalIllumination>();
            ref var camera = ref entity.Get<PerspectiveCameraComponent>();

            if (config.TracingBuffer == null)
                config.TracingBuffer = CreateIlluminationBuffer();
            if (config.SampleBuffer == null)
                config.SampleBuffer = CreateIlluminationBuffer();
            if (config.BlurBufferA == null)
                config.BlurBufferA = CreateIlluminationBuffer();
            if (config.BlurBufferB == null)
                config.BlurBufferB = CreateIlluminationBuffer();

            if (config.TracingBuffer.Width != camera.DeferredLightBuffer.Width / config.SampleBufferLength || config.TracingBuffer.Height != camera.DeferredLightBuffer.Height / config.SampleBufferLength)
            {
                config.TracingBuffer.Handle = 0;
                config.TracingBuffer.Width = camera.DeferredLightBuffer.Width / config.SampleBufferLength;
                config.TracingBuffer.Height = camera.DeferredLightBuffer.Height / config.SampleBufferLength;
            }

            if (config.SampleBuffer.Width != camera.DeferredLightBuffer.Width || config.SampleBuffer.Height != camera.DeferredLightBuffer.Height)
            {
                config.SampleBuffer.Handle = 0;
                config.SampleBuffer.Width = camera.DeferredLightBuffer.Width;
                config.SampleBuffer.Height = camera.DeferredLightBuffer.Height;
            }

            if (config.BlurBufferA.Width != camera.DeferredLightBuffer.Width || config.BlurBufferA.Height != camera.DeferredLightBuffer.Height)
            {
                config.BlurBufferA.Handle = 0;
                config.BlurBufferA.Width = camera.DeferredLightBuffer.Width;
                config.BlurBufferA.Height = camera.DeferredLightBuffer.Height;
            }

            if (config.BlurBufferB.Width != camera.DeferredLightBuffer.Width || config.BlurBufferB.Height != camera.DeferredLightBuffer.Height)
            {
                config.BlurBufferB.Handle = 0;
                config.BlurBufferB.Width = camera.DeferredLightBuffer.Width;
                config.BlurBufferB.Height = camera.DeferredLightBuffer.Height;
            }

            // TRACE GI
            _postTraceMaterial.SetUniform("LightMap", camera.DeferredLightBuffer.Textures[0]);
            _postTraceMaterial.SetUniform("ViewPosition", camera.ShaderViewSpace.ViewPosition);
            _postTraceMaterial.SetUniform("ViewDirection", camera.ShaderViewSpace.ViewDirection);
            _postTraceMaterial.SetUniform("Projection", camera.ShaderViewSpace.WorldToViewRotation);
            foreach (var texture in camera.DeferredGBuffer.Textures)
                _postTraceMaterial.SetUniform(texture.Name, texture);

            Renderer.Use(config.TracingBuffer);
            Renderer.Use(Defaults.Shader.Program.PostGlobalIllumination);
            Renderer.Use(_postTraceMaterial, Defaults.Shader.Program.PostGlobalIllumination);

            GL.Viewport(0, 0, config.TracingBuffer.Width, config.TracingBuffer.Height);
            GL.ClearColor(config.TracingBuffer.ClearColor);
            GL.Clear(config.TracingBuffer.ClearMask);

            Renderer.Draw(Defaults.Vertex.Mesh.Plane[0]);


            // SAVE SAMPLES
            _sampleBufferIndexX++;
            if (_sampleBufferIndexX == config.SampleBufferLength)
            {
                _sampleBufferIndexX = 0;
                _sampleBufferIndexY++;
            }
            if (_sampleBufferIndexY == config.SampleBufferLength)
            {
                _sampleBufferIndexY = 0;
            }

            _postSampleMaterial.SetUniform("TraceMap", config.TracingBuffer.Textures[0]);
            _postSampleMaterial.SetUniform("BufferLength", 4);
            _postSampleMaterial.SetUniform("IndexX", _sampleBufferIndexX);
            _postSampleMaterial.SetUniform("IndexY", _sampleBufferIndexY);

            Renderer.Use(config.SampleBuffer);
            Renderer.Use(Defaults.Shader.Program.PostGlobalIlluminationStorage);
            Renderer.Use(_postSampleMaterial, Defaults.Shader.Program.PostGlobalIlluminationStorage);

            GL.Viewport(0, 0, config.SampleBuffer.Width, config.SampleBuffer.Height);

            Renderer.Draw(Defaults.Vertex.Mesh.Plane[0]);


            // BLUR SAMPLES
            _postBlurMaterial.SetUniform("BufferMap", config.SampleBuffer.Textures[0]);
            _postBlurMaterial.SetUniform("Horizontal", 0f);

            Renderer.Use(config.BlurBufferA);
            Renderer.Use(Defaults.Shader.Program.PostGaussianBlur);
            Renderer.Use(_postBlurMaterial, Defaults.Shader.Program.PostGaussianBlur);

            GL.Viewport(0, 0, config.BlurBufferA.Width, config.BlurBufferA.Height);

            Renderer.Draw(Defaults.Vertex.Mesh.Plane[0]);

            _postBlurMaterial.SetUniform("BufferMap", config.BlurBufferA.Textures[0]);
            _postBlurMaterial.SetUniform("Horizontal", 1f);

            Renderer.Use(config.BlurBufferB);
            Renderer.Use(Defaults.Shader.Program.PostGaussianBlur);
            Renderer.Use(_postBlurMaterial, Defaults.Shader.Program.PostGaussianBlur);

            GL.Viewport(0, 0, config.BlurBufferB.Width, config.BlurBufferB.Height);

            Renderer.Draw(Defaults.Vertex.Mesh.Plane[0]);


            // COPY RESULT
            Renderer.Blit(config.BlurBufferB, camera.DeferredLightBuffer, ClearBufferMask.ColorBufferBit);
        }

        /// <summary>
        /// 
        /// </summary>
        private FramebufferAsset CreateIlluminationBuffer()
        {
            return new FramebufferAsset("IlluminationBuffer")
            {
                DrawTargets = new DrawBuffersEnum[] { DrawBuffersEnum.ColorAttachment0 },
                Textures = new List<TextureRenderAsset>()
                {
                    new TextureRenderAsset("Illumination")
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
