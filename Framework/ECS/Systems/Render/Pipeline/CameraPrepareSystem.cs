using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Shader.Block;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Scene;
using Framework.ECS.Components.Transform;
using Framework.ECS.Systems.Render.OpenGL;
using OpenTK.Mathematics;

namespace Framework.ECS.Systems.Render.Pipeline
{
    [With(typeof(TransformComponent))]
    [With(typeof(PerspectiveCameraComponent))]
    public class CameraPrepareSystem : AEntitySetSystem<bool>
    {
        private readonly Entity _worldComponents;

        /// <summary>
        /// 
        /// </summary>
        public CameraPrepareSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
        }

        protected override void Update(bool state, in Entity entity)
        {
            ref var transform = ref entity.Get<TransformComponent>();
            ref var camera = ref entity.Get<PerspectiveCameraComponent>();
            ref var aspect = ref _worldComponents.Get<AspectRatioComponent>();

            if (camera.ShaderViewSpace == null)
                camera.ShaderViewSpace = new ShaderViewSpaceBlock();

            if (camera.ShaderDeferredView == null)
                camera.ShaderDeferredView = new ShaderDeferredViewBlock();

            if (camera.DeferredGBuffer == null)
                camera.DeferredGBuffer = Defaults.Framebuffer.CreateDeferredGBuffer();

            if (camera.DeferredLightBuffer == null)
                camera.DeferredLightBuffer = Defaults.Framebuffer.CreateDeferredLightBuffer("DeferredCameraResult");

            
            if (camera.DeferredGBuffer.Width != aspect.Width || camera.DeferredGBuffer.Height != aspect.Height)
            {
                camera.DeferredGBuffer.Handle = 0;
                camera.DeferredGBuffer.Width = aspect.Width;
                camera.DeferredGBuffer.Height = aspect.Height;

                GPUSync.Push(camera.DeferredGBuffer);
            }

            if (camera.DeferredLightBuffer.Width != aspect.Width || camera.DeferredLightBuffer.Height != aspect.Height)
            {
                camera.DeferredLightBuffer.Handle = 0;
                camera.DeferredLightBuffer.Width = aspect.Width;
                camera.DeferredLightBuffer.Height = aspect.Height;

                GPUSync.Push(camera.DeferredLightBuffer);
            }

            camera.ShaderViewSpace.Projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(camera.FieldOfView),
                aspect.Ratio,
                camera.NearClipping,
                camera.FarClipping
            );

            camera.ShaderViewSpace.WorldToView = transform.WorldSpaceInverse;
            camera.ShaderViewSpace.WorldToProjection = transform.WorldSpaceInverse * camera.ShaderViewSpace.Projection;
            camera.ShaderViewSpace.WorldToViewRotation = transform.WorldSpaceInverse.ClearScale().ClearTranslation();
            camera.ShaderViewSpace.WorldToProjectionRotation = transform.WorldSpaceInverse.ClearScale().ClearTranslation() * camera.ShaderViewSpace.Projection;
            camera.ShaderViewSpace.ViewPosition = new Vector4(transform.Position, 1);
            camera.ShaderViewSpace.ViewDirection = new Vector4(transform.Forward, 0);
            camera.ShaderViewSpace.Resolution = new Vector2(aspect.Width, aspect.Height);
            
            GPUSync.Push(camera.ShaderViewSpace);
        }
    }
}
