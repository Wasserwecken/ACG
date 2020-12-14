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
        public VertexBuffer Buffer { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public VertexObject(BufferUsageHint usageHint)
        {
            UsageHint = usageHint;

            Indicies = new uint[0];

            Buffer = new VertexBuffer(BufferTarget.ArrayBuffer, usageHint, VertexAttribPointerType.Float);
        }

        /// <summary>
        /// 
        /// </summary>
        public void AddVertices(float[] vertices)
        {
            AddAttribute(
                Definitions.Shader.Attributes.Vertices.LAYOUT,
                Definitions.Buffer.Vertices.PAIRLENGTH,
                Definitions.Buffer.Vertices.NORMALIZED,
                Definitions.Buffer.Vertices.POINTERTYPE,
                vertices
            );
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void AddNormals(float[] normals)
        {
            AddAttribute(
                Definitions.Shader.Attributes.Normals.LAYOUT,
                Definitions.Buffer.Normals.PAIRLENGTH,
                Definitions.Buffer.Normals.NORMALIZED,
                Definitions.Buffer.Normals.POINTERTYPE,
                normals
            );
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void AddColors(float[] colors)
        {
            AddAttribute(
                Definitions.Shader.Attributes.Colors.LAYOUT,
                Definitions.Buffer.Colors.PAIRLENGTH,
                Definitions.Buffer.Colors.NORMALIZED,
                Definitions.Buffer.Colors.POINTERTYPE,
                colors
            );
        }

        /// <summary>
        /// 
        /// </summary>
        public void AddUV(float[] uv)
        {
            AddAttribute(
                Definitions.Shader.Attributes.UV.LAYOUT,
                Definitions.Buffer.UV.PAIRLENGTH,
                Definitions.Buffer.UV.NORMALIZED,
                Definitions.Buffer.UV.POINTERTYPE,
                uv
            );
        }

        /// <summary>
        /// 
        /// </summary>
        public void AddAttribute<TType>(int layout, int pairLength, bool isNormalized, VertexAttribPointerType pointerType, TType[] data) where TType : struct
        {
            Buffer.AddAttribute(VertexAttribute.Create(layout, pairLength, isNormalized, pointerType, data));
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
                GL.DrawArrays(PrimitiveType.Triangles, 0, Buffer.ArrayBuffer.Length);

            GL.BindVertexArray(0);
        }
    }
}
