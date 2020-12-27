using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Framework
{
    public struct RenderData
    {
        public Matrix4 WorldToProjectionSpace { get; set; }
        public Matrix4 WorldToViewSpace { get; set; }
        public Matrix3 WorldToViewRotationSpace { get; set; }

        public int LocalToProjectionSpaceLayout { get; set; }
        public int LocalToViewSpaceLayout { get; set; }
        public int LocalToViewRotationSpaceLayout { get; set; }
        public int LocalToWorldSpaceLayout { get; set; }
        public int LocalToWorldRotationSpaceLayout { get; set; }

        public Vector3 ViewPosition { get; set; }
        public float TimeDelta { get; set; }
        public float TimeTotal { get; set; }
    }
}
