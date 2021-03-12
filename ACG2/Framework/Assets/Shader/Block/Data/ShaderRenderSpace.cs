using OpenTK.Mathematics;

namespace Framework
{
    public struct ShaderRenderSpace
    {
        public Matrix4 LocalToWorld;
        public Matrix4 LocalToView;
        public Matrix4 LocalToProjection;
        public Matrix4 LocalToWorldRotation;
        public Matrix4 LocalToViewRotation;
        public Matrix4 WorldToView;
        public Vector4 ViewPosition;
        public Vector4 ViewDirection;
    }
}
