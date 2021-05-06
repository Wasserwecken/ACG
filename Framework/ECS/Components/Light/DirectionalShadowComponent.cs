using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.ECS.Components.Light
{
    public struct DirectionalShadowComponent
    {
        public int Resolution;
        public float Strength;
        public float NearClipping;
        public float FarClipping;
        public float Width;
    }
}
