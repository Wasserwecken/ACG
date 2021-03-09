using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Framework.ECS.Components.Render
{
    [DebuggerDisplay("FieldOfView: {FieldOfView}, Clipping: {NearClipping} - {FarClipping}")]
    public class PerspectiveCameraComponent : IComponent
    {
        public Vector4 ClearColor { get; set; }
        public ClearBufferMask ClearMask { get; set; }
        public float NearClipping { get; set; }
        public float FarClipping { get; set; }
        public float FieldOfView { get; set; }
    }
}
