using System.Diagnostics;
using OpenTK.Graphics.OpenGL;

namespace Framework.Assets.Verticies
{
    [DebuggerDisplay("Name: {Name}, Handle: {Handle}, ElementCount: {ElementCount}, ElementSize: {ElementSize}, UsageHint: {UsageHint}")]
    public abstract class BufferBaseAsset
    {
        public int Handle { get; set; }
        public string Name { get; }
        public int ElementSize { get; }
        public BufferTarget Target { get; }
        public int ElementCount => Data.Length / ElementSize;
        public BufferUsageHint UsageHint { get; set; }
        public byte[] Data { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public BufferBaseAsset(BufferUsageHint usageHint, BufferTarget target, string name, int elementSize)
        {
            Name = name;
            ElementSize = elementSize;
            UsageHint = usageHint;
            Target = target;
            Data = new byte[0];
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void PushToGPU()
        {
            if (Handle <= 0)
                Handle = GL.GenBuffer();

            GL.BindBuffer(Target, Handle);
            GL.BufferData(Target, Data.Length, Data, UsageHint);
        }
    }
}
