using Framework.ECS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Framework.ECS.Components
{
    [DebuggerDisplay("Ratio: {Ratio}, Width: {Width}, Height: {Height}")]
    public class AspectRatioComponent : IComponent
    {
        public float Width { get; set; }
        public float Height { get; set; }
        public float Ratio { get; set; }
    }
}
