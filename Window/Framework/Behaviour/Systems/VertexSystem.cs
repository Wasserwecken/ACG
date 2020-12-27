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


            var localToWorldSpace = transformData.Space;
            GL.UniformMatrix4(renderData.LocalToWorldSpaceLayout, false, ref localToWorldSpace);

            var localToWorldRotationSpace = transformData.RotationSpace;
            GL.UniformMatrix3(renderData.LocalToWorldRotationSpaceLayout, false, ref localToWorldRotationSpace);


            var localToViewSpace = transformData.Space * renderData.WorldToViewSpace;
            GL.UniformMatrix4(renderData.LocalToViewSpaceLayout, false, ref localToViewSpace);

            var localToViewRotationSpace = transformData.RotationSpace * renderData.WorldToViewRotationSpace;
            GL.UniformMatrix3(renderData.LocalToViewRotationSpaceLayout, false, ref localToViewRotationSpace);


            var localToProjectionSpace = localToWorldSpace * renderData.WorldToProjectionSpace;
            GL.UniformMatrix4(renderData.LocalToProjectionSpaceLayout, false, ref localToProjectionSpace);


            if (vertexData.ObjectData.IsIndexed)
                GL.DrawElements(vertexData.Primitive, vertexData.ObjectData.Indicies.Length, DrawElementsType.UnsignedInt, 0);
            else
                GL.DrawArrays(vertexData.Primitive, 0, vertexData.ObjectData.Buffer.ArrayBuffer.Length);
        }
    }
}
