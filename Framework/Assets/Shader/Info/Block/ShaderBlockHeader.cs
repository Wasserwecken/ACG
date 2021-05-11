using OpenTK.Graphics.OpenGL;

namespace Framework.Assets.Shader.Info.Block
{
    public struct ShaderBlockHeader
    {
        public int Handle;
        public string Name;
        public BufferRangeTarget Target;
        public BufferUsageHint UsageHint;
    }
}
