using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public class ShaderUniformBlock<TBlockType> where TBlockType : struct
    {
        public static int BlockSize = Marshal.SizeOf(typeof(TBlockType));
        public int Handle { get; set; }
        public BufferUsageHint UsageHint { get; set; }
        public TBlockType Data;

        /// <summary>
        /// 
        /// </summary>
        public ShaderUniformBlock(BufferUsageHint usageHint)
        {
            Handle = -1;
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
