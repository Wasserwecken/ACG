using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public struct PerspectiveCameraData
    {
        public TransformData Transform { get; set; }
        public Vector4 ClearColor { get; set; }
        public ClearBufferMask ClearMask { get; set; }
        public float NearClipping { get; set; }
        public float FarClipping { get; set; }
        public float AspectRatio { get; set; }

        public float FieldOfView { get; set; }
    }
}
