using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;
using Framework.Assets.Shader.Block;
using Framework.Assets.Shader.Block.Data;

namespace Framework.ECS.Components.Render
{
    [DebuggerDisplay("FieldOfView: {FieldOfView}, Clipping: {NearClipping} - {FarClipping}")]
    public struct PerspectiveCameraComponent
    {
        public ShaderBlockSingle<ShaderViewSpace> ShaderViewSpace;
        public Vector4 ClearColor;
        public ClearBufferMask ClearMask;
        public float NearClipping;
        public float FarClipping;
        public float FieldOfView;
    }
}
