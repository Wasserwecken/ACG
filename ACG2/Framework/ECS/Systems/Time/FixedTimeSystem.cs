using Framework.ECS.Components.Scene;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Framework.ECS.Systems.Time
{
    public class FixedTimeSystem : ISystem
    {
        private readonly Stopwatch _watch;

        /// <summary>
        /// 
        /// </summary>
        public FixedTimeSystem()
        {
            _watch = new Stopwatch();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Run(IEnumerable<Entity> entities, IEnumerable<IComponent> sceneComponents)
        {
            var timeComponent = sceneComponents.First(f => f is TimeComponent) as TimeComponent;
            timeComponent.DeltaFixed = (float)_watch.Elapsed.TotalSeconds;

            _watch.Restart();
        }
    }
}
