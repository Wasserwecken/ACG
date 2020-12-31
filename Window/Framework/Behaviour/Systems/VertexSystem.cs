using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public class VertexSystem
    {
        public static void Draw(VertexData vertexData)
        {
            GL.BindVertexArray(vertexData.ObjectData.VertexHandle);
            GL.ShadeModel(vertexData.Shading);

            if (vertexData.ObjectData.IsIndexed)
                GL.DrawElements(vertexData.Primitive, vertexData.ObjectData.Indicies.Length, DrawElementsType.UnsignedInt, 0);
            else
                GL.DrawArrays(vertexData.Primitive, 0, vertexData.ObjectData.Buffer.ArrayBuffer.Length);
        }
    }
}
