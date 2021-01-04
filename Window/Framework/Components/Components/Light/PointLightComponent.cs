using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public struct PointLightComponent : IEntityComponent
    {
        public Vector3 Color;
        public float AmbientFactor;
    }
}
