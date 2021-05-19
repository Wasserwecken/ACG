using ACG.Framework.Assets;
using DefaultEcs;
using DefaultEcs.System;
using Framework.ECS.Components.Scene;
using Framework.ECS.Systems.Render.OpenGL;

namespace Framework.ECS.Systems.Render.Pipeline
{
    class ReflectionBufferSyncSystem : AComponentSystem<bool, ReflectionBufferComponent>
    {
        /// <summary>
        /// 
        /// </summary>
        public ReflectionBufferSyncSystem(World world, Entity worldComponents) : base(world)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, ref ReflectionBufferComponent component)
        {
            GPUSync.Push(component.ReflectionBlock);

            foreach (var shader in AssetRegister.Shaders)
            {
                shader.SetBlockBinding(component.ReflectionBlock);

                foreach (var texture in component.DeferredLightBuffer.Textures)
                    shader.SetTextureBinding(texture);
            }
        }
    }
}
