using Framework.Assets.Shader.Block;
using Framework.ECS.Components.Scene;
using DefaultEcs.System;
using DefaultEcs;
using ACG.Framework.Assets;
using Framework.ECS.Systems.Render.OpenGL;

namespace Framework.ECS.Systems.Sync
{
    public class ShaderTimeSystem : AComponentSystem<bool, TimeComponent>
    {
        private readonly Entity _worldComponents;
        private readonly ShaderTimeBlock _block;

        /// <summary>
        /// 
        /// </summary>
        public ShaderTimeSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
            _block = new ShaderTimeBlock();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, ref TimeComponent time)
        {
            _block.Time = time;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void PostUpdate(bool state)
        {
            GPUSync.Push(_block);
            
            foreach (var shader in AssetRegister.Shaders)
                shader.SetBlockBinding(_block);
        }
    }
}
