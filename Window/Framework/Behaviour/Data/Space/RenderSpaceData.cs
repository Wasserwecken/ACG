using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public struct RenderSpaceData
    {
        public Matrix4 LocalToWorld;
        public Matrix4 LocalToView;
        public Matrix4 LocalToProjection;

        public Matrix4 LocalToWorldRotation;
        public Matrix4 LocalToViewRotation;
    }
}
