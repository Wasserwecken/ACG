using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public static class IndicieBufferSystem
    {
        /// <summary>
        /// 
        /// </summary>
        public static void Update(uint[] indicies, IndicieBufferAsset buffer)
        {
            buffer.Data = new byte[indicies.Length * buffer.ElementSize];
            System.Buffer.BlockCopy(indicies, 0, buffer.Data, 0, buffer.ElementSize * indicies.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void PushToGPU(IndicieBufferAsset buffer)
        {
            buffer.Handle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, buffer.Handle);
            GL.BufferData(BufferTarget.ElementArrayBuffer, buffer.Data.Length, buffer.Data, buffer.UsageHint);
        }
    }
}
