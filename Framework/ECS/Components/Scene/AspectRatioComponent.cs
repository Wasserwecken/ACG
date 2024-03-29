﻿using System.Diagnostics;

namespace Framework.ECS.Components.Scene
{
    [DebuggerDisplay("Ratio: {Ratio}, Width: {Width}, Height: {Height}")]
    public struct AspectRatioComponent
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public float Ratio => Width / (float)Height;
    }
}
