using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public struct SpaceData
    {
        public Matrix4 LocalToWorld;
        public Matrix4 LocalToView;
        public Matrix4 LocalToProjection;

        public Matrix3 LocalToWorldRotation;
        public Matrix3 LocalToViewRotation;
    }
}
