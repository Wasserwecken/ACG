﻿using OpenTK.Mathematics;
using System.Diagnostics;

namespace Framework.ECS.Components.Light
{
    [DebuggerDisplay("Color: {Color}, AmbientFactor: {AmbientFactor}")]
    public class DirectionalLightComponent : IComponent
    {
        public Vector3 Color;
        public float AmbientFactor;
        public float ShadowStrength;
    }
}
