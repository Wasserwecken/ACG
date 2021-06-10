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
    [With(typeof(PostAmbientOcclusionComponent))]
    public class CameraPostGlobalIllumination : AEntitySetSystem<bool>
    {
        private readonly Entity _worldComponents;
        private readonly MaterialAsset _postMaterial;

        /// <summary>
        /// 
        /// </summary>
        public CameraPostGlobalIllumination(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
            _postMaterial = new MaterialAsset("PostIllumination") { IsWritingDepth = false, DepthTest = DepthFunction.Always };
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, in Entity entity)
        {
            ref var config = ref entity.Get<PostTonemappingComponent>();
            ref var camera = ref entity.Get<PerspectiveCameraComponent>();

            if (config.Buffer == null)
                config.Buffer = CreateIlluminationBuffer();

            if (config.Buffer.Width != camera.DeferredLightBuffer.Width || config.Buffer.Height != camera.DeferredLightBuffer.Height)
            {
                config.Buffer.Handle = 0;
                config.Buffer.Width = camera.DeferredLightBuffer.Width;
                config.Buffer.Height = camera.DeferredLightBuffer.Height;
            }

            _postMaterial.SetUniform("BufferMap", camera.DeferredLightBuffer.Textures[0]);
            _postMaterial.SetUniform("ViewPosition", camera.ShaderViewSpace.ViewPosition);
            _postMaterial.SetUniform("Projection", camera.ShaderViewSpace.WorldToProjection);
            foreach (var texture in camera.DeferredGBuffer.Textures)
                _postMaterial.SetUniform(texture.Name, texture);

            GL.Viewport(0, 0, camera.DeferredLightBuffer.Width, camera.DeferredLightBuffer.Height);

            // RENDER
            Renderer.Use(config.Buffer);
            Renderer.Use(Defaults.Shader.Program.PostGlobalIllumination);
            Renderer.Use(_postMaterial, Defaults.Shader.Program.PostGlobalIllumination);
            Renderer.Draw(Defaults.Vertex.Mesh.Plane[0]);

            // COPY RESULT
            Renderer.Blit(config.Buffer, camera.DeferredLightBuffer, ClearBufferMask.ColorBufferBit);
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
