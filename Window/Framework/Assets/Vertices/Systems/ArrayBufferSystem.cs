using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public static class ArrayBufferSystem
    {
        /// <summary>
        /// 
        /// </summary>
        public static void UpdateData(ArrayBufferAsset buffer)
        {
            buffer.Data = new byte[buffer.Attributes[0].ElementCount * buffer.ElementSize];
            for (int i = 0; i < buffer.ElementCount; i++)
            {
                var bufferIndex = i * buffer.ElementSize;
                foreach (var attribute in buffer.Attributes)
                {
                    var attributeIndex = i * attribute.ElementSize;
                    System.Buffer.BlockCopy(attribute.Data, attributeIndex, buffer.Data, bufferIndex, attribute.ElementSize);
                    bufferIndex += attribute.ElementSize;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void PushToGPU(ArrayBufferAsset buffer)
        {
            BufferBaseSystem.PushToGPU(buffer);

            var offset = 0;
            foreach (var attribute in buffer.Attributes)
            {
                GL.VertexAttribPointer(attribute.Layout, attribute.Dimension, attribute.PointerType, attribute.IsNormalized, buffer.ElementSize, offset);
                GL.EnableVertexAttribArray(attribute.Layout);
                offset += attribute.ElementSize;
            }
        }
    }
}
