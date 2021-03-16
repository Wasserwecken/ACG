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
        private ShaderBlockSingle<ShaderTime> _timeBlock;

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
            var time = sceneComponents.First(f => f is TimeComponent) as TimeComponent;
            _timeBlock.Data.Total = time.Total;
            _timeBlock.Data.TotalSin = time.TotalSin;
            _timeBlock.Data.Frame = time.DeltaFrame;
            _timeBlock.Data.Fixed = time.DeltaFixed;
            _timeBlock.PushToGPU();
        }
    }
}
