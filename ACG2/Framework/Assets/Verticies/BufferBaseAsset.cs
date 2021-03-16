using System.Diagnostics;
using OpenTK.Graphics.OpenGL;

namespace Framework.Assets.Verticies
{
    [DebuggerDisplay("Name: {Name}, Handle: {Handle}, ElementSize: {ElementSize}, UsageHint: {UsageHint}")]
    public abstract class BufferBaseAsset
    {
        public int Handle { get; set; }
        public string Name { get; }
        public int ElementSize { get; }
        public BufferTarget Target { get; }
        public BufferUsageHint UsageHint { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public BufferBaseAsset(BufferUsageHint usageHint, BufferTarget target, string name, int elementSize)
        {
            Name = name;
            ElementSize = elementSize;
            UsageHint = usageHint;
            Target = target;
        }
    }
}
