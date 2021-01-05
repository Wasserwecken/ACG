using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public static class BufferBaseSystem
    {
        /// <summary>
        /// 
        /// </summary>
        public static void PushToGPU(BufferBase buffer)
        {
            if (buffer.Handle <= 0)
                buffer.Handle = GL.GenBuffer();

            GL.BindBuffer(buffer.Target, buffer.Handle);
            GL.BufferData(buffer.Target, buffer.Data.Length, buffer.Data, buffer.UsageHint);
            //GL.BindBuffer(buffer.Target, 0);
        }
    }
}
