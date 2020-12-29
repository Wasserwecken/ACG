using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public class UniformBlock<TBlockType> : IUniformBlock where TBlockType : struct
    {
        public static int BlockSize = Marshal.SizeOf(typeof(TBlockType));
        public int Handle { get; private set; }
        public string Name { get; private set; }
        public BufferUsageHint UsageHint { get; set; }
        public TBlockType Data;

        /// <summary>
        /// 
        /// </summary>
        public UniformBlock(string name, BufferUsageHint usageHint)
        {
            Handle = -1;
            Name = name;
            Data = default;
            UsageHint = usageHint;
        }

        /// <summary>
        /// 
        /// </summary>
        public void PushToGPU()
        {
            if (Handle < 0)
                Handle = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.UniformBuffer, Handle);
            GL.BufferData(BufferTarget.UniformBuffer, BlockSize, ref Data, UsageHint);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }
    }
}
