using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public struct ShaderSpace
    {
        public Matrix4 LocalToWorld;
        public Matrix4 LocalToView;
        public Matrix4 LocalToProjection;
        public Matrix4 LocalToWorldRotation;
        public Matrix4 LocalToViewRotation;
        public Matrix4 WorldToView;
        public Vector4 ViewPosition;
        public Vector4 ViewDirection;
    }
}
