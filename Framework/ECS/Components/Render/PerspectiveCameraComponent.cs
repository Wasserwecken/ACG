using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;
using Framework.Assets.Shader.Block;

namespace Framework.ECS.Components.Render
{
    [DebuggerDisplay("FieldOfView: {FieldOfView}, Clipping: {NearClipping} - {FarClipping}")]
    public struct PerspectiveCameraComponent
    {
        public ShaderViewSpaceBlock ShaderViewSpace;
        public Vector4 ClearColor;
        public ClearBufferMask ClearMask;
        public float NearClipping;
        public float FarClipping;
        public float FieldOfView;
    }
}
