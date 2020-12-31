using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public class VertexBuffer
    {
        public int BufferHandle { get; private set; }
        public BufferUsageHint UsageHint { get; set; }
        public int StrideSize { get; private set; }
        public byte[] ArrayBuffer { get; private set; }
        public List<VertexAttribute> Attributes { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        public VertexBuffer(VertexAttribPointerType attribPointerType)
        {
            UsageHint = BufferUsageHint.StaticDraw;
            Attributes = new List<VertexAttribute>();
        }

        /// <summary>
        /// 
        /// </summary>
        public void AddAttribute(VertexAttribute attribute)
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
                strideLength += attribute.PairSize;
            }

            ArrayBuffer = new byte[bufferLength];
            StrideSize = strideLength;

            for (int i = 0; i < vertexCount; i++)
            {
                var bufferIndex = i * strideLength;

                foreach (var attribute in Attributes)
                {
                    var attributeIndex = i * attribute.PairSize;
                    System.Buffer.BlockCopy(attribute.Data, attributeIndex, ArrayBuffer, bufferIndex, attribute.PairSize);
                    bufferIndex += attribute.PairSize;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void PushToGPU()
        {
            BufferHandle = GL.GenBuffer();
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, BufferHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, ArrayBuffer.Length, ArrayBuffer, UsageHint);

            var offset = 0;
            foreach (var attribute in Attributes)
            {
                GL.VertexAttribPointer(attribute.Layout, attribute.PairLength, attribute.PointerType, attribute.IsNormalized, StrideSize, offset);
                GL.EnableVertexAttribArray(attribute.Layout);
                offset += attribute.PairSize;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
