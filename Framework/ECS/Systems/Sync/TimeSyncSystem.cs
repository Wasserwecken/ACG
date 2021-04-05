using Framework.Assets.Shader.Block;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using System.Linq;
using Framework.ECS.Components.Scene;
using Framework.Assets.Shader.Block.Data;

namespace Framework.ECS.Systems.Sync
{
    public class TimeSyncSystem : ISystem
    {
        private readonly ShaderBlockSingle<ShaderTime> _timeBlock;

        /// <summary>
        /// 
        /// </summary>
        public TimeSyncSystem()
        {
            _timeBlock = new ShaderBlockSingle<ShaderTime>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Run(IEnumerable<Entity> entities, IEnumerable<IComponent> sceneComponents)
        {
            if (sceneComponents.TryGet<TimeComponent>(out var time))
            {
                _timeBlock.Data.Total = time.Total;
                _timeBlock.Data.TotalSin = time.TotalSin;
                _timeBlock.Data.Frame = time.DeltaFrame;
                _timeBlock.Data.Fixed = time.DeltaFixed;
                _timeBlock.PushToGPU();
            }
        }
    }
}
