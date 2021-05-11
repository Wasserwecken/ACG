using Framework.Assets.Shader.Block;
using OpenTK.Graphics.OpenGL;
using Framework.ECS.Components.Scene;
using Framework.Assets.Shader.Block.Data;
using DefaultEcs.System;
using DefaultEcs;
using Framework.ECS.Components.Render;

namespace Framework.ECS.Systems.Sync
{
    public class TimeSyncSystem : AComponentSystem<bool, TimeComponent>
    {
        private readonly Entity _worldComponents;

        /// <summary>
        /// 
        /// </summary>
        public TimeSyncSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, ref TimeComponent timeComponent)
        {
            ref var shaderBlocks = ref _worldComponents.Get<GlobalShaderBlocksComponent>();

            shaderBlocks.Time.Data.Total = timeComponent.Total;
            shaderBlocks.Time.Data.TotalSin = timeComponent.TotalSin;
            shaderBlocks.Time.Data.Frame = timeComponent.DeltaFrame;
            shaderBlocks.Time.Data.Fixed = timeComponent.DeltaFixed;
            shaderBlocks.Time.PushToGPU();
        }
    }
}
