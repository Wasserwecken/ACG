using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.ECS.Components.Light
{
    public struct ReflectionProbeComponent
    {
        public bool HasChanged;
        public int Resolution;
        public float NearClipping;
        public float FarClipping;
    }
}
