using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public class VertexObject
    {
        public int VertexHandle { get; private set; }
        public bool IsIndexed => Indicies.Length > 0;
        public int IndicieHandle { get; private set; }
        public uint[] Indicies { get; private set; }
        public BufferUsageHint UsageHint { get; private set; }
        public VertexBuffer<float> Buffer { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public VertexObject(BufferUsageHint usageHint)
        {
            UsageHint = usageHint;

            Indicies = new uint[0];
            Buffer = new VertexBuffer<float>(BufferTarget.ArrayBuffer, usageHint, VertexAttribPointerType.Float);
        }

        /// <summary>
        /// 
        /// </summary>
        public void AddAttribute(VertexAttribute<float> attribute)
        {
            Buffer.AddAttribute(attribute);
        }

        /// <summary>
        /// 
        /// </summary>
        public void AddIndicies(uint[] indicies)
        {
            Indicies = indicies;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Prepare()
        {
            Buffer.Prepare();
        }

        /// <summary>
        /// 
        /// </summary>
        public void PushToGPU()
        {
            VertexHandle = GL.GenVertexArray();
            GL.BindVertexArray(VertexHandle);

            Buffer.PushToGPU();

            if (IsIndexed)
            {
                IndicieHandle = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndicieHandle);
                GL.BufferData(BufferTarget.ElementArrayBuffer, Indicies.Length * sizeof(uint), Indicies, UsageHint);
            }

            GL.BindVertexArray(0);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Draw()
        {
            GL.BindVertexArray(VertexHandle);

            if (IsIndexed)
                GL.DrawElements(PrimitiveType.Triangles, Indicies.Length, DrawElementsType.UnsignedInt, 0);
            else
                GL.DrawArrays(PrimitiveType.Triangles, 0, Buffer.BufferData.Length);

            GL.BindVertexArray(0);
        }
    }
}
