using DefaultEcs;
using DefaultEcs.System;
using Framework.ECS.Components.Scene;
using System.Diagnostics;

namespace Framework.ECS.Systems.Time
{
    public class FixedTimeSystem : AComponentSystem<bool, TimeComponent>
    {
        private readonly Stopwatch _watch;

        /// <summary>
        /// 
        /// </summary>
        public FixedTimeSystem(World world) : base(world)
        {
            _watch = new Stopwatch();
            _watch.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, ref TimeComponent timeComponent)
        {
            timeComponent.DeltaFixed = (float)_watch.Elapsed.TotalSeconds;
            _watch.Restart();
        }
    }
}
