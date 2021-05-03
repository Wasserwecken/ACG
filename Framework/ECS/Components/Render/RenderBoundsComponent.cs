using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Framework.ECS.Components.Render
{
    public struct RenderBoundsComponent
    {
        public bool IsVisible;
        public Vector3 BoundsCenter;
        public Vector3 BoundsSize;
        public Vector3 BoundsMin;
        public Vector3 BoundsMax;
    }
}
