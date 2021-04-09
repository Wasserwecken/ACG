using Framework.ECS.Components.Scene;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Framework.ECS.Systems.Time
{
    public class TotalTimeSystem : ISystem
    {
        private readonly Stopwatch _watch;

        /// <summary>
        /// 
        /// </summary>
        public TotalTimeSystem()
        {
            _watch = new Stopwatch();
            _watch.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Run(IEnumerable<Entity> entities, IEnumerable<IComponent> sceneComponents)
        {
            var timeComponent = sceneComponents.First(f => f is TimeComponent) as TimeComponent;
            timeComponent.Total = (float)_watch.Elapsed.TotalSeconds;
            timeComponent.TotalSin = MathF.Sin(timeComponent.Total);
        }
    }
}
