using Framework.ECS.Components.Scene;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using Framework.Assets.Verticies;

namespace Framework.ECS.Systems
{
    public class PrimitiveSyncSystem : ISystem
    {
        /// <summary>
        /// 
        /// </summary>
        public void Update(IEnumerable<Entity> entities, IEnumerable<IComponent> sceneComponents)
        {
            var renderGraphComponent = sceneComponents.First(f => f is RenderGraphComponent) as RenderGraphComponent;

            foreach(var primitive in renderGraphComponent.Primitves)
            {
                if (primitive.Handle <= 0)
                {
                    // creating buffer bytes
                    var arrayBuffer = CreateBufferArrayData(primitive.ArrayBuffer);
                    var indicieBuffer = CreateBufferIndicieData(primitive.IndicieBuffer);
                    
                    // GPU buffer reservation
                    primitive.Handle = GL.GenVertexArray();
                    primitive.ArrayBuffer.Handle = GL.GenBuffer();
                    
                    // send arraybuffer to GPU
                    GL.BindVertexArray(primitive.Handle);
                    GL.BindBuffer(primitive.ArrayBuffer.Target, primitive.ArrayBuffer.Handle);
                    GL.BufferData(primitive.ArrayBuffer.Target, arrayBuffer.Length, arrayBuffer, primitive.ArrayBuffer.UsageHint);

                    // send attribute infos to GPU
                    var offset = 0;
                    foreach (var attribute in primitive.ArrayBuffer.Attributes)
                    {
                        GL.VertexAttribPointer(attribute.Layout, attribute.Dimension, attribute.PointerType, attribute.IsNormalized, primitive.ArrayBuffer.ElementSize, offset);
                        GL.EnableVertexAttribArray(attribute.Layout);
                        offset += attribute.ElementSize;
                    }

                    // send indicie info to GPU
                    if (primitive.IndicieBuffer != null)
                    {
                        primitive.IndicieBuffer.Handle = GL.GenBuffer();
                        GL.BindBuffer(primitive.IndicieBuffer.Target, primitive.IndicieBuffer.Handle);
                        GL.BufferData(primitive.IndicieBuffer.Target, indicieBuffer.Length, indicieBuffer, primitive.IndicieBuffer.UsageHint);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private byte[] CreateBufferArrayData(BufferArrayAsset buffer)
        {
            // set buffer size
            var result = new byte[buffer.Attributes[0].ElementCount * buffer.ElementSize];
            
            // go through each data element
            var elementCount = buffer.Attributes[0].ElementCount;
            for (int i = 0; i < elementCount; i++)
            {
                var bufferIndex = i * buffer.ElementSize;

                // copy atribute data for a single element into the buffer object
                foreach (var attribute in buffer.Attributes)
                {
                    var attributeIndex = i * attribute.ElementSize;
                    System.Buffer.BlockCopy(attribute.Data, attributeIndex, result, bufferIndex, attribute.ElementSize);
                    bufferIndex += attribute.ElementSize;
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        private byte[] CreateBufferIndicieData(BufferIndicieAsset buffer)
        {
            return buffer.Indicies.ToBytes();
        }
    }
}
