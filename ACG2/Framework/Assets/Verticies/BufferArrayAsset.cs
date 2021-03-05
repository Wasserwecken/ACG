using System.Linq;
using OpenTK.Graphics.OpenGL;

namespace Framework.Assets.Verticies
{
    public class BufferArrayAsset : BufferBaseAsset
    {
        public VertexAttributeAsset[] Attributes { get; }

        /// <summary>
        /// 
        /// </summary>
        public BufferArrayAsset(BufferUsageHint usageHint)
            : this(usageHint, new VertexAttributeAsset[0]) { }

        /// <summary>
        /// 
        /// </summary>
        public BufferArrayAsset(BufferUsageHint usageHint, VertexAttributeAsset[] attributes)
            : base(usageHint, BufferTarget.ArrayBuffer, "VertexArray", attributes.Sum(a => a.ElementSize))
        {
            Attributes = attributes;
            CreateBufferData();
        }

        /// <summary>
        /// 
        /// </summary>
        public void CreateBufferData()
        {
            Data = new byte[Attributes[0].ElementCount * ElementSize];
            for (int i = 0; i < ElementCount; i++)
            {
                var bufferIndex = i * ElementSize;
                foreach (var attribute in Attributes)
                {
                    var attributeIndex = i * attribute.ElementSize;
                    System.Buffer.BlockCopy(attribute.Data, attributeIndex, Data, bufferIndex, attribute.ElementSize);
                    bufferIndex += attribute.ElementSize;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void PushToGPU()
        {
            base.PushToGPU();

            var offset = 0;
            foreach (var attribute in Attributes)
            {
                GL.VertexAttribPointer(attribute.Layout, attribute.Dimension, attribute.PointerType, attribute.IsNormalized, ElementSize, offset);
                GL.EnableVertexAttribArray(attribute.Layout);
                offset += attribute.ElementSize;
            }
        }
    }
}
