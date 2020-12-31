﻿using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public class VertexObject
    {
        public int VertexHandle { get; protected set; }
        public VertexBuffer Buffer { get; protected set; }
        public BufferUsageHint UsageHint { get; set; }
        public PolygonMode Mode { get; set; }

        public int IndicieHandle { get; protected set; }
        public bool IsIndexed => Indicies.Length > 0;
        public uint[] Indicies { get; protected set; }


        /// <summary>
        /// 
        /// </summary>
        public VertexObject()
        {
            Indicies = new uint[0];
            Mode = PolygonMode.Fill;
            UsageHint = BufferUsageHint.StaticDraw;
            Buffer = new VertexBuffer(VertexAttribPointerType.Float);
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
        public void AddVertices<TType>(ICollection<TType> vertices, Func<TType, float[]> toFloats)
        {
            AddAttribute(
                Definitions.Shader.Attributes.Vertices.LAYOUT,
                Definitions.Buffer.Vertices.PAIRLENGTH,
                Definitions.Buffer.Vertices.NORMALIZED,
                Definitions.Buffer.Vertices.POINTERTYPE,
                vertices, toFloats
            );
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
        public void AddAttribute<TContainer, TType>(int layout, int pairLength, bool isNormalized, VertexAttribPointerType pointerType, ICollection<TContainer> dataList, Func<TContainer, TType[]> toTypeArray) where TType : struct
        {
            var typeList = new List<TType>();
            foreach (var entry in dataList)
                typeList.AddRange(toTypeArray(entry));

            AddAttribute(layout, pairLength, isNormalized, pointerType, typeList.ToArray());
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
        public virtual void Prepare()
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

            PushObjectToGPU();

            GL.BindVertexArray(0);
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void PushObjectToGPU()
        {
            Buffer.UsageHint = UsageHint;
            Buffer.PushToGPU();

            GL.PolygonMode(MaterialFace.FrontAndBack, Mode);

            if (IsIndexed)
            {
                IndicieHandle = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndicieHandle);
                GL.BufferData(BufferTarget.ElementArrayBuffer, Indicies.Length * sizeof(uint), Indicies, UsageHint);
            }
        }
    }
}
