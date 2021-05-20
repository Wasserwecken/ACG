using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Shader.Block;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Scene;
using Framework.ECS.Systems.Render.OpenGL;

namespace Framework.ECS.Systems.Render.Pipeline
{
    public class CameraPrepareSystem : AComponentSystem<bool, PerspectiveCameraComponent>
    {
        private readonly Entity _worldComponents;

        /// <summary>
        /// 
        /// </summary>
        public CameraPrepareSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, ref PerspectiveCameraComponent component)
        {
            var aspect = _worldComponents.Get<AspectRatioComponent>();

            if (component.ShaderViewSpace == null)
                component.ShaderViewSpace = new ShaderViewSpaceBlock();

            if (component.ShaderDeferredView == null)
                component.ShaderDeferredView = new ShaderDeferredViewBlock();

            if (component.DeferredGBuffer == null)
                component.DeferredGBuffer = Defaults.Framebuffer.CreateDeferredGBuffer();

            if (component.DeferredLightBuffer == null)
                component.DeferredLightBuffer = Defaults.Framebuffer.CreateDeferredLightBuffer("DeferredCameraResult");

            if (component.DeferredGBuffer.Width != aspect.Width || component.DeferredGBuffer.Height != aspect.Height)
            {
                component.DeferredGBuffer.Handle = 0;
                component.DeferredGBuffer.Width = aspect.Width;
                component.DeferredGBuffer.Height = aspect.Height;

                GPUSync.Push(component.DeferredGBuffer);
            }

            if (component.DeferredLightBuffer.Width != aspect.Width || component.DeferredLightBuffer.Height != aspect.Height)
            {
                component.DeferredLightBuffer.Handle = 0;
                component.DeferredLightBuffer.Width = aspect.Width;
                component.DeferredLightBuffer.Height = aspect.Height;

                GPUSync.Push(component.DeferredLightBuffer);
            }
        }
    }
}
