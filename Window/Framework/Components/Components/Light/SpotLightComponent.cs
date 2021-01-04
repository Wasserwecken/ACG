using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public struct SpotLightComponent : IEntityComponent
    {
        public Vector3 Color;
        public float AmbientFactor;
        public float OuterAngle;
        public float InnerAngle;
    }
}
