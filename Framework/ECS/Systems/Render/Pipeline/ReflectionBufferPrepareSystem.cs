using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Shader.Block;
using Framework.Assets.Textures;
using Framework.ECS.Components.Scene;
using Framework.ECS.Systems.Render.OpenGL;

namespace Framework.ECS.Systems.Render.Pipeline
{
    public class ReflectionBufferPrepareSystem : AComponentSystem<bool, ReflectionBufferComponent>
    {
        /// <summary>
        /// 
        /// </summary>
        public ReflectionBufferPrepareSystem(World world, Entity worldComponents) : base(world)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, ref ReflectionBufferComponent component)
        {
            if (component.TextureAtlas == null || component.TextureAtlas.Resolution != component.Size)
                component.TextureAtlas = new TextureSpace(component.Size);
            component.TextureAtlas.Clear();

            component.ReflectionBlock.Probes = new ShaderReflectionBlock.ShaderReflectionProbe[0];
            GPUSync.Push(component.ReflectionBlock);

            component.DeferredGBuffer.Width = component.Size;
            component.DeferredGBuffer.Height = component.Size;

            component.DeferredLightBuffer.Width = component.Size;
            component.DeferredLightBuffer.Height = component.Size;
        }
    }
}
