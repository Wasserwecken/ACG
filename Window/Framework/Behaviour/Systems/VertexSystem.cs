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


            if (renderData.SpaceBlockLayout >= 0)
            {
                renderData.SpaceBlock.Data.LocalToWorld = transformData.Space;
                renderData.SpaceBlock.Data.LocalToProjection = transformData.Space * renderData.SpaceBlock.Data.LocalToProjection;
                renderData.SpaceBlock.PushToGPU();
                GL.BindBufferBase(BufferRangeTarget.UniformBuffer, renderData.SpaceBlockLayout, renderData.SpaceBlock.Handle);
            }


            if (vertexData.ObjectData.IsIndexed)
                GL.DrawElements(vertexData.Primitive, vertexData.ObjectData.Indicies.Length, DrawElementsType.UnsignedInt, 0);
            else
                GL.DrawArrays(vertexData.Primitive, 0, vertexData.ObjectData.Buffer.ArrayBuffer.Length);
        }
    }
}
