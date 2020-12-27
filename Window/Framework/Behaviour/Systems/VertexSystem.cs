using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public class VertexSystem
    {
        public static void Draw(TransformData transformData, VertexData vertexData, RenderData renderData)
        {
            GL.BindVertexArray(vertexData.ObjectData.VertexHandle);
            GL.ShadeModel(vertexData.Shading);

            var localToWorld = transformData.Space;
            GL.UniformMatrix4(renderData.LocalToWorldLayout, false, ref localToWorld);
            var localToProjection = localToWorld * renderData.WorldToProjection;
            GL.UniformMatrix4(renderData.LocalToProjectionLayout, false, ref localToProjection);

            if (vertexData.ObjectData.IsIndexed)
                GL.DrawElements(vertexData.Primitive, vertexData.ObjectData.Indicies.Length, DrawElementsType.UnsignedInt, 0);
            else
                GL.DrawArrays(vertexData.Primitive, 0, vertexData.ObjectData.Buffer.ArrayBuffer.Length);
        }
    }
}
