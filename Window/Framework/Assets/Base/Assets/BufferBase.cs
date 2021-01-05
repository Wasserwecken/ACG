using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    [DebuggerDisplay("Name: {Name}, Handle: {Handle}, ElementCount: {ElementCount}, ElementSize: {ElementSize}, UsageHint: {UsageHint}")]
    public abstract class BufferBase
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
        public BufferBase(string name, int elementSize, BufferTarget target, BufferUsageHint usageHint)
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
        public void SetData<TType>(TType[] data)
        {
            var typeSize = Marshal.SizeOf<TType>();
            Data = new byte[data.Length * typeSize];
            System.Buffer.BlockCopy(data, 0, Data, 0, Data.Length);
        }
    }
}
