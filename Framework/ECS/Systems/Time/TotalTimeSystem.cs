﻿using DefaultEcs;
using DefaultEcs.System;
using Framework.ECS.Components.Scene;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Framework.ECS.Systems.Time
{
    public class TotalTimeSystem : AComponentSystem<bool, TimeComponent>
    {
        private readonly Stopwatch _watch;

        /// <summary>
        /// 
        /// </summary>
        public TotalTimeSystem(World world) : base(world)
        {
            _watch = new Stopwatch();
            _watch.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, ref TimeComponent timeComponent)
        {
            timeComponent.Total = (float)_watch.Elapsed.TotalSeconds;
            timeComponent.TotalSin = MathF.Sin(timeComponent.Total);
        }
    }
}
