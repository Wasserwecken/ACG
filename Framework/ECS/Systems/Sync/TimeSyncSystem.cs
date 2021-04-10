using Framework.Assets.Shader.Block;
using OpenTK.Graphics.OpenGL;
using Framework.ECS.Components.Scene;
using Framework.Assets.Shader.Block.Data;
using DefaultEcs.System;
using DefaultEcs;

namespace Framework.ECS.Systems.Sync
{
    public class TimeSyncSystem : AComponentSystem<bool, TimeComponent>
    {
        private readonly ShaderBlockSingle<ShaderTime> _timeBlock;

        /// <summary>
        /// 
        /// </summary>
        public TimeSyncSystem(World world) : base(world)
        {
            _timeBlock = new ShaderBlockSingle<ShaderTime>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, ref TimeComponent timeComponent)
        {
            _timeBlock.Data.Total = timeComponent.Total;
            _timeBlock.Data.TotalSin = timeComponent.TotalSin;
            _timeBlock.Data.Frame = timeComponent.DeltaFrame;
            _timeBlock.Data.Fixed = timeComponent.DeltaFixed;
            _timeBlock.PushToGPU();
        }
    }
}
