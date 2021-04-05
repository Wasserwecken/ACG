using OpenTK.Mathematics;

namespace Framework.Assets.Shader.Block.Data
{
    public struct ShaderViewSpace
    {
        public Matrix4 WorldToView;
        public Matrix4 WorldToProjection;
        public Matrix4 WorldToViewRotation;
        public Matrix4 WorldToProjectionRotation;
        public Vector4 ViewPosition;
        public Vector4 ViewDirection;
    }
}
