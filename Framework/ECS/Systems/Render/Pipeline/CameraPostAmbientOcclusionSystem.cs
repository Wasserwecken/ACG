using ACG.Framework.Assets;
using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Framebuffer;
using Framework.Assets.Materials;
using Framework.Assets.Textures;
using Framework.ECS.Components.PostProcessing;
using Framework.ECS.Components.Render;
using Framework.ECS.Systems.Render.OpenGL;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.ECS.Systems.Render.Pipeline
{
    [With(typeof(PerspectiveCameraComponent))]
    [With(typeof(PostAmbientOcclusionComponent))]
    public class CameraPostAmbientOcclusionSystem : AEntitySetSystem<bool>
    {
        private readonly Entity _worldComponents;
        private readonly MaterialAsset _occulsionMaterial;
        private readonly MaterialAsset _mergeMaterial;
        private readonly MaterialAsset _blurMaterial;

        /// <summary>
        /// 
        /// </summary>
        public CameraPostAmbientOcclusionSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
            _occulsionMaterial = new MaterialAsset("PostOcclusionSelect") { IsWritingDepth = false, DepthTest = DepthFunction.Always };
            _mergeMaterial = new MaterialAsset("PostOcclusionMerge") { IsWritingDepth = false, DepthTest = DepthFunction.Always };
            _blurMaterial = new MaterialAsset("PostBlur") { IsWritingDepth = false, DepthTest = DepthFunction.Always };
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, in Entity entity)
        {
            ref var config = ref entity.Get<PostAmbientOcclusionComponent>();
            ref var camera = ref entity.Get<PerspectiveCameraComponent>();


            if (config.BufferA == null)
                config.BufferA = Defaults.Framebuffer.CreateDeferredAmbientOcclusionBuffer();

            if (config.BufferB == null)
                config.BufferB = Defaults.Framebuffer.CreateDeferredAmbientOcclusionBuffer();

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

            // SELECT OCCLUSION
            _occulsionMaterial.SetUniform("Bias", config.Bias);
            _occulsionMaterial.SetUniform("Radius", config.Radius);
            _occulsionMaterial.SetUniform("Strength", config.Strength);
            _occulsionMaterial.SetUniform("ViewPosition", camera.ShaderViewSpace.ViewPosition);
            _occulsionMaterial.SetUniform("Projection", camera.ShaderViewSpace.WorldToProjection);

            foreach (var texture in camera.DeferredGBuffer.Textures)
                _occulsionMaterial.SetUniform(texture.Name, texture);

            Renderer.Use(config.BufferA);
            Renderer.Use(Defaults.Shader.Program.PostAmbientOcclusionSelect);
            Renderer.Use(_occulsionMaterial, Defaults.Shader.Program.PostAmbientOcclusionSelect);
            Renderer.Draw(Defaults.Vertex.Mesh.Plane[0]);

            // BLUR
            Renderer.Use(Defaults.Shader.Program.PostGaussianBlur);
            for (int i = 0; i < 1; i++)
            {
                _blurMaterial.SetUniform("BufferMap", config.BufferA.Textures[0]);
                _blurMaterial.SetUniform("Horizontal", 0f);

                Renderer.Use(config.BufferB);
                Renderer.Use(_blurMaterial, Defaults.Shader.Program.PostGaussianBlur);
                Renderer.Draw(Defaults.Vertex.Mesh.Plane[0]);


                Renderer.Use(config.BufferA);
                _blurMaterial.SetUniform("BufferMap", config.BufferB.Textures[0]);
                _blurMaterial.SetUniform("Horizontal", 1f);

                Renderer.Use(_blurMaterial, Defaults.Shader.Program.PostGaussianBlur);
                Renderer.Draw(Defaults.Vertex.Mesh.Plane[0]);
            }

            // MERGE
            _mergeMaterial.SetUniform("BufferMap", camera.DeferredLightBuffer.Textures[0]);
            _mergeMaterial.SetUniform("AmbientOcclusionMap", config.BufferA.Textures[0]);

            Renderer.Use(config.BufferB);
            Renderer.Use(Defaults.Shader.Program.PostAmbientOcclusionMerge);
            Renderer.Use(_mergeMaterial, Defaults.Shader.Program.PostAmbientOcclusionMerge);
            Renderer.Draw(Defaults.Vertex.Mesh.Plane[0]);

            // COPY
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, config.BufferB.Handle);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, camera.DeferredLightBuffer.Handle);
            GL.BlitFramebuffer(
                0, 0, config.BufferB.Width, config.BufferB.Height,
                0, 0, camera.DeferredLightBuffer.Width, camera.DeferredLightBuffer.Height,
                ClearBufferMask.ColorBufferBit,
                BlitFramebufferFilter.Nearest
            );
        }
    }
}
