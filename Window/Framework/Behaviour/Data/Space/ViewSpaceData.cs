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
        public Matrix3 WorldToViewRotation;
    }
}
