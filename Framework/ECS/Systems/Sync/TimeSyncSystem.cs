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

        /// <summary>
        /// 
        /// </summary>
        public TimeSyncSystem(World world, Entity worldComponents) : base(world)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, ref TimeComponent timeComponent)
        {
            ShaderBlockSingle<ShaderTime>.Instance.Data.Total = timeComponent.Total;
            ShaderBlockSingle<ShaderTime>.Instance.Data.TotalSin = timeComponent.TotalSin;
            ShaderBlockSingle<ShaderTime>.Instance.Data.Frame = timeComponent.DeltaFrame;
            ShaderBlockSingle<ShaderTime>.Instance.Data.Fixed = timeComponent.DeltaFixed;
            ShaderBlockSingle<ShaderTime>.Instance.PushToGPU();
        }
    }
}
