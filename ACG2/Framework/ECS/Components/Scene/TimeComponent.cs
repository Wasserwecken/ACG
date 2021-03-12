using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.ECS.Components.Scene
{
    public class TimeComponent : IComponent
    {
        public float Total { get; set; }
        public float TotalSin { get; set; }
        public float DeltaFrame { get; set; }
        public float DeltaFixed { get; set; }
    }
}
