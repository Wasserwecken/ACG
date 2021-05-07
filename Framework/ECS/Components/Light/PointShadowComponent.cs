﻿using System.Diagnostics;

namespace Framework.ECS.Components.Light
{
    [DebuggerDisplay("Resolution: {Resolution}, Strength: {StrengthCount}")]
    public struct PointShadowComponent
    {
        public int Resolution;
        public float Strength;
        public float NearClipping;
    }
}