using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public interface IShaderBlock
    {
        public int Handle { get; }
        public string Name { get; }
        public BufferRangeTarget Target { get; }
        public BufferUsageHint UsageHint { get; set; }

        public void PushToGPU();
    }
}
