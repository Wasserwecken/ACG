using OpenTK.Mathematics;

namespace Framework.Assets.Shader.Block.Data
{
    public struct ShaderDirectionalLight
    {
        public Vector4 Color;
        public Vector4 Direction;
        public Vector4 ShadowArea;
        public Matrix4 ShadowSpace;
        public Vector4 ShadowStrength;
    }
}
