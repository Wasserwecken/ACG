using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public class VertexBuffer
    {
        public int BufferHandle { get; private set; }

        public BufferTarget Target { get; private set; }
        public BufferUsageHint UsageHint { get; private set; }
        public VertexAttribPointerType AttribPointerType { get; private set; }

        public int StrideSize { get; private set; }
        public int BufferSize { get; private set; }
        public int TypeSize { get; private set; }

        public byte[] BufferData { get; private set; }
        public List<VertexAttribute<TType>> Attributes { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        public VertexBuffer(BufferTarget target, BufferUsageHint usageHint, VertexAttribPointerType attribPointerType)
        {
            Target = target;
            UsageHint = usageHint;
            AttribPointerType = attribPointerType;

            TypeSize = Marshal.SizeOf(default(TType));
            Attributes = new List<VertexAttribute<TType>>();
        }

        /// <summary>
        /// 
        /// </summary>
        public void AddAttribute(VertexAttribute<TType> attribute)
        {
            if (!Attributes.Contains(attribute))
                Attributes.Add(attribute);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Prepare()
        {
            var vertexCount = Attributes[0].PairCount;
            var bufferLength = 0;
            var strideLength = 0;

            foreach (var attribute in Attributes)
            {
                bufferLength += attribute.Data.Length;
                strideLength += attribute.PairLength;
            }

            BufferData = new byte[bufferLength];
            BufferSize = bufferLength * TypeSize;
            StrideSize = strideLength * TypeSize;

            for (int i = 0; i < vertexCount; i++)
            {
                var bufferIndex = i * strideLength;

                foreach (var attribute in Attributes)
                {
                    var attributeIndex = i * attribute.PairLength;

                    System.Buffer.BlockCopy()

                    Array.Copy(attribute.Data, attributeIndex, BufferData, bufferIndex, attribute.PairLength);

                    bufferIndex += attribute.PairLength;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void PushToGPU()
        {
            BufferHandle = GL.GenBuffer();
            
            GL.BindBuffer(Target, BufferHandle);
            GL.BufferData(Target, BufferSize, BufferData, UsageHint);

            var offset = 0;
            foreach (var attribute in Attributes)
            {
                GL.VertexAttribPointer(attribute.Layout, attribute.PairLength, AttribPointerType, attribute.IsNormalized, StrideSize, offset);
                GL.EnableVertexAttribArray(attribute.Layout);
                offset += attribute.PairLength * TypeSize;
            }

            GL.BindBuffer(Target, 0);
        }
    }
}
