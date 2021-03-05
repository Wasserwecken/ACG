using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public static class VertexPrimitiveManager
    {
        /// <summary>
        /// 
        /// </summary>
        public static void PushToGPU(VertexPrimitiveAsset primitive)
        {
            if (primitive.Handle <= 0)
                primitive.Handle = GL.GenVertexArray();
            
            GL.BindVertexArray(primitive.Handle);

            ArrayBufferManager.UpdateBufferData(primitive.ArrayBuffer);
            ArrayBufferManager.PushToGPU(primitive.ArrayBuffer);

            if (primitive.IndicieBuffer != null)
                BufferBaseManager.PushToGPU(primitive.IndicieBuffer);

            GL.BindVertexArray(0);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Draw(VertexPrimitiveAsset primitive)
        {
            GL.BindVertexArray(primitive.Handle);
            GL.PolygonMode(MaterialFace.FrontAndBack, primitive.Mode);

            if (primitive.IndicieBuffer != null)
                GL.DrawElements(primitive.Type, primitive.IndicieBuffer.ElementCount, DrawElementsType.UnsignedInt, 0);
            else
                GL.DrawArrays(primitive.Type, 0, primitive.ArrayBuffer.ElementCount);
        }
    }
}
