using ACG.Framework.Assets;
using DefaultEcs;
using DefaultEcs.System;
using Framework.ECS.Components.Scene;
using Framework.ECS.Systems.Render.OpenGL;

namespace Framework.ECS.Systems.Render.Pipeline
{
    class ShadowBufferSyncSystem : AComponentSystem<bool, ShadowBufferComponent>
    {
        /// <summary>
        /// 
        /// </summary>
        public ShadowBufferSyncSystem(World world, Entity worldComponents) : base(world)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, ref ShadowBufferComponent component)
        {
            GPUSync.Push(component.DirectionalBlock);
            GPUSync.Push(component.PointBlock);
            GPUSync.Push(component.SpotBlock);

            foreach (var shader in AssetRegister.Shaders)
                shader.SetBlockBinding(component.DirectionalBlock);

            foreach (var shader in AssetRegister.Shaders)
                shader.SetBlockBinding(component.PointBlock);

            foreach (var shader in AssetRegister.Shaders)
                shader.SetBlockBinding(component.SpotBlock);
        }
    }
}
