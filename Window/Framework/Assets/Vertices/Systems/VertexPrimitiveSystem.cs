using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public static class VertexPrimitiveSystem
    {
        /// <summary>
        /// 
        /// </summary>
        public static void PushToGPU(VertexPrimitiveAsset primitive)
        {
            primitive.Handle = GL.GenVertexArray();
            GL.BindVertexArray(primitive.Handle);

            GL.PolygonMode(MaterialFace.FrontAndBack, primitive.Mode);
            
            ArrayBufferSystem.PushToGPU(primitive.ArrayBuffer);
            if (primitive.IndicieBuffer != null)
                IndicieBufferSystem.PushToGPU(primitive.IndicieBuffer);

            GL.BindVertexArray(0);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Draw(VertexPrimitiveAsset primitive)
        {
            GL.BindVertexArray(primitive.Handle);

            if (primitive.IndicieBuffer != null)
                GL.DrawElements(primitive.Type, primitive.IndicieBuffer.ElementCount, DrawElementsType.UnsignedInt, primitive.IndicieBuffer.Data);
            else
                GL.DrawArrays(primitive.Type, 0, primitive.ArrayBuffer.ElementCount);
        }
    }
}
