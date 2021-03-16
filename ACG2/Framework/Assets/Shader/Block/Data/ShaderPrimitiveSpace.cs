using OpenTK.Mathematics;

namespace Framework.Assets.Shader.Block.Data
{
    public struct ShaderPrimitiveSpace
    {
        public Matrix4 LocalToWorld;
        public Matrix4 LocalToView;
        public Matrix4 LocalToProjection;
        public Matrix4 LocalToWorldRotation;
        public Matrix4 LocalToViewRotation;
    }
}
