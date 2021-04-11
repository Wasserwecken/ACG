using Framework.ECS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Framework.ECS.Components.Scene
{
    [DebuggerDisplay("Ratio: {Ratio}, Width: {Width}, Height: {Height}")]
    public struct AspectRatioComponent
    {
        public float Width { get; set; }
        public float Height { get; set; }
        public float Ratio => Width / Height;
    }
}
