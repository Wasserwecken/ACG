using Framework.ECS.Components.Scene;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Framework.ECS.Systems.Time
{
    public class FrameTimeSystem : ISystem
    {
        private readonly Stopwatch _watch;

        /// <summary>
        /// 
        /// </summary>
        public FrameTimeSystem()
        {
            _watch = new Stopwatch();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Run(IEnumerable<Entity> entities, IEnumerable<IComponent> sceneComponents)
        {
            var timeComponent = sceneComponents.First(f => f is TimeComponent) as TimeComponent;
            timeComponent.DeltaFrame = (float)_watch.Elapsed.TotalSeconds;

            _watch.Restart();
        }
    }
}
