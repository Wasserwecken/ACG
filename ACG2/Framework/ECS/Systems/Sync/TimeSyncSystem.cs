using Framework.Assets.Shader.Block;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using System.Linq;
using Framework.ECS.Components.Scene;

namespace Framework.ECS.Systems.Sync
{
    public class TimeSyncSystem : ISystem
    {
        private ShaderBlockSingle<ShaderTime> _timeUniformBlock;

        /// <summary>
        /// 
        /// </summary>
        public TimeSyncSystem()
        {
            _timeUniformBlock = new ShaderBlockSingle<ShaderTime>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Run(IEnumerable<Entity> entities, IEnumerable<IComponent> sceneComponents)
        {
            var time = sceneComponents.First(f => f is TimeComponent) as TimeComponent;
            _timeUniformBlock.Data.Total = time.Total;
            _timeUniformBlock.Data.TotalSin = time.TotalSin;
            _timeUniformBlock.Data.Frame = time.DeltaFrame;
            _timeUniformBlock.Data.Fixed = time.DeltaFixed;
            _timeUniformBlock.PushToGPU();
        }
    }
}
