using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.ECS.Components.Light
{
    public struct ShadowCasterComponent
    {
        public float NearClipping;
        public float FarClipping;
    }
}
