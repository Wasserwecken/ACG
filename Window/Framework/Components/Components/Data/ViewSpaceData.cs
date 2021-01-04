using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public struct ViewSpaceData
    {
        public Matrix4 WorldToView;
        public Matrix4 WorldToProjection;
        public Matrix4 WorldToViewRotation;
        public Vector3 ViewPosition;
        public Vector3 ViewDirection;
    }
}
