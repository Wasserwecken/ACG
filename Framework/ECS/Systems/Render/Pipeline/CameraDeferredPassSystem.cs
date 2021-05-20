using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Materials;
using Framework.Assets.Shader.Block;
using Framework.ECS.Components.Light;
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
    public class CameraDeferredPassSystem : AEntitySetSystem<bool>
    {
        private readonly Entity _worldComponents;
        private readonly EntitySet _renderCandidates;
        private readonly MaterialAsset _lightMaterial;

        /// <summary>
        /// 
        /// </summary>
        public CameraDeferredPassSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
            _renderCandidates = World.GetEntities()
                .With<TransformComponent>()
                .With<PrimitiveComponent>()
                .AsSet();

            _lightMaterial = new MaterialAsset("DeferredLight") { DepthTest = DepthFunction.Always };
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, in Entity entity)
        {
            var aspect = _worldComponents.Get<AspectRatioComponent>();
            var transform = entity.Get<TransformComponent>();
            var camera = entity.Get<PerspectiveCameraComponent>();


            // G BUFFER
            var projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(camera.FieldOfView),
                aspect.Ratio,
                camera.NearClipping,
                camera.FarClipping
            );
            camera.ShaderViewSpace.WorldToView = transform.WorldSpaceInverse;
            camera.ShaderViewSpace.WorldToProjection = transform.WorldSpaceInverse * projection;
            camera.ShaderViewSpace.WorldToViewRotation = transform.WorldSpaceInverse.ClearScale().ClearTranslation();
            camera.ShaderViewSpace.WorldToProjectionRotation = transform.WorldSpaceInverse.ClearScale().ClearTranslation() * projection;
            camera.ShaderViewSpace.ViewPosition = new Vector4(transform.Position, 1);
            camera.ShaderViewSpace.ViewDirection = new Vector4(transform.Forward, 0);
            camera.ShaderViewSpace.Resolution = new Vector2(aspect.Width, aspect.Height);
            GPUSync.Push(camera.ShaderViewSpace);

            Renderer.Use(camera.DeferredGBuffer);
            Renderer.Use(Defaults.Shader.Program.MeshLitDeferredBuffer);
            Renderer.Use(camera.ShaderViewSpace, Defaults.Shader.Program.MeshLitDeferredBuffer);

            GL.Viewport(0, 0, camera.DeferredGBuffer.Width, camera.DeferredGBuffer.Height);
            GL.ClearColor(camera.DeferredGBuffer.ClearColor);
            GL.Clear(camera.DeferredGBuffer.ClearMask);

            foreach (ref readonly var candidate in _renderCandidates.GetEntities())
            {
                var primitive = candidate.Get<PrimitiveComponent>();

                Renderer.Use(primitive.Material, Defaults.Shader.Program.MeshLitDeferredBuffer);
                Renderer.Use(primitive.PrimitiveSpaceBlock, Defaults.Shader.Program.MeshLitDeferredBuffer);
                Renderer.Draw(primitive.Verticies);
            }


            // LIGHT BUFFER
            camera.ShaderDeferredView.ViewPosition = new Vector4(transform.Position, 1f);
            camera.ShaderDeferredView.ViewPort = new Vector4(0, 0, aspect.Width, aspect.Height);
            camera.ShaderDeferredView.Resolution = new Vector2(aspect.Width, aspect.Height);
            GPUSync.Push(camera.ShaderDeferredView);

            Renderer.Use(camera.DeferredLightBuffer);
            Renderer.Use(Defaults.Shader.Program.MeshLitDeferredLight);
            Renderer.Use(camera.ShaderDeferredView, Defaults.Shader.Program.MeshLitDeferredLight);

            GL.Viewport(0, 0, camera.DeferredLightBuffer.Width, camera.DeferredLightBuffer.Height);
            GL.ClearColor(camera.DeferredLightBuffer.ClearColor);
            GL.Clear(camera.DeferredLightBuffer.ClearMask);

            _lightMaterial.SetUniform("SkyboxMap", camera.Skybox);
            foreach (var texture in camera.DeferredGBuffer.Textures)
                _lightMaterial.SetUniform(texture.Name, texture);

            Renderer.Use(_lightMaterial, Defaults.Shader.Program.MeshLitDeferredLight);
            Renderer.Use(camera.ShaderDeferredView, Defaults.Shader.Program.MeshLitDeferredLight);
            Renderer.Draw(Defaults.Vertex.Mesh.Plane[0]);


            // BLIT DEPTH
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, camera.DeferredGBuffer.Handle);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, camera.DeferredLightBuffer.Handle);
            GL.BlitFramebuffer(
                0, 0, camera.DeferredGBuffer.Width, camera.DeferredGBuffer.Height,
                0, 0, camera.DeferredLightBuffer.Width, camera.DeferredLightBuffer.Height,
                ClearBufferMask.DepthBufferBit,
                BlitFramebufferFilter.Nearest
            );


            // DEV
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, camera.DeferredLightBuffer.Handle);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
            GL.BlitFramebuffer(
                0, 0, camera.DeferredLightBuffer.Width, camera.DeferredLightBuffer.Height,
                0, 0, camera.DeferredLightBuffer.Width, camera.DeferredLightBuffer.Height,
                ClearBufferMask.ColorBufferBit,
                BlitFramebufferFilter.Nearest
            );
        }
    }
}
