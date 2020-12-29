using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public struct ObjectSpaceData
    {
        public Matrix4 LocalToWorld;
        public Matrix3 LocalToWorldRotation;
    }
}
