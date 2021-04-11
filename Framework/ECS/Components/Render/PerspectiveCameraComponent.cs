using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;

namespace Framework.ECS.Components.Render
{
    [DebuggerDisplay("FieldOfView: {FieldOfView}, Clipping: {NearClipping} - {FarClipping}")]
    public struct PerspectiveCameraComponent
    {
        public Vector4 ClearColor { get; set; }
        public ClearBufferMask ClearMask { get; set; }
        public float NearClipping { get; set; }
        public float FarClipping { get; set; }
        public float FieldOfView { get; set; }
    }
}
