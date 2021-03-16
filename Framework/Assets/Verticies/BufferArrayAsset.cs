using System.Linq;
using OpenTK.Graphics.OpenGL;

namespace Framework.Assets.Verticies
{
    public class BufferArrayAsset : BufferBaseAsset
    {
        public override int ElementCount => Attributes[0].ElementCount;
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
        }
    }
}
