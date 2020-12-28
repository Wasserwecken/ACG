using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public class ShaderStorageBuffer<TBlockType> where TBlockType : struct
    {
        public static int BlockSize = Marshal.SizeOf(typeof(TBlockType));
        public int Handle { get; set; }
        public TBlockType Data { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ShaderStorageBuffer(TBlockType data)
        {
            Data = data;
        }

        /// <summary>
        /// 
        /// </summary>
        public void PushToGPU()
        {
            var data = Data;

            Handle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.UniformBuffer, Handle);
            GL.BufferData(BufferTarget.UniformBuffer, BlockSize, ref data, BufferUsageHint.DynamicDraw);
        }
    }
}
