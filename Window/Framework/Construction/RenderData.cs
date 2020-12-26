using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Framework
{
    public struct RenderData
    {
        public Matrix4 WorldToProjection { get; set; }
        public Matrix4 LocalToWorld { get; set; }
        public Matrix4 LocalToProjection { get; set; }

        public Vector3 ViewPosition { get; set; }

        public float TimeDelta { get; set; }
        public float TimeTotal { get; set; }
    }
}
