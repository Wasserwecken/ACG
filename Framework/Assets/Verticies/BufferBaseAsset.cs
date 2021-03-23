using System.Diagnostics;
using OpenTK.Graphics.OpenGL;

namespace Framework.Assets.Verticies
{
    [DebuggerDisplay("Name: {Name}, Handle: {Handle}, ElementCount: {ElementCount}, ElementSize: {ElementSize}, UsageHint: {UsageHint}")]
    public abstract class BufferBaseAsset
    {
        public int Handle { get; set; }
        public string Name { get; }
        public BufferTarget Target { get; }
        public BufferUsageHint UsageHint { get; set; }
        public abstract int ElementSize { get; }
        public abstract int ElementCount { get; }

        /// <summary>
        /// 
        /// </summary>
        public BufferBaseAsset(BufferUsageHint usageHint, BufferTarget target, string name)
        {
            Name = name;
            UsageHint = usageHint;
            Target = target;
        }
    }
}
