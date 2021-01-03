using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public int ElementCount { get; set; }
        public BufferUsageHint UsageHint { get; set; }
        public byte[] Data { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public BufferBase(string name, int elementSize, BufferUsageHint usageHint)
        {
            Name = name;
            ElementSize = elementSize;
            UsageHint = usageHint;
            Data = new byte[0];
        }
    }
}
