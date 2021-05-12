using Framework.Assets.Shader.Block;
using OpenTK.Graphics.OpenGL;
using Framework.ECS.Components.Scene;
using Framework.Assets.Shader.Block.Data;
using DefaultEcs.System;
using DefaultEcs;
using Framework.ECS.Components.Render;
using System.IO;
using System;
using Framework.ECS.Systems.Render;
using ACG.Framework.Assets;

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
