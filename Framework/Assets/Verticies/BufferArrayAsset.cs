using System.Linq;
using Framework.Assets.Verticies.Attributes;
using OpenTK.Graphics.OpenGL;

namespace Framework.Assets.Verticies
{
    public class BufferArrayAsset : BufferBaseAsset
    {
        public override int ElementCount => Attributes[0].ElementCount;
        public IVertexAttribute[] Attributes { get; }

        /// <summary>
        /// 
        /// </summary>
        public BufferArrayAsset(BufferUsageHint usageHint)
            : this(usageHint, new IVertexAttribute[0]) { }

        /// <summary>
        /// 
        /// </summary>
        public BufferArrayAsset(BufferUsageHint usageHint, IVertexAttribute[] attributes)
            : base(usageHint, BufferTarget.ArrayBuffer, "VertexArray", attributes.Sum(a => a.ElementSize))
        {
            Attributes = attributes;
        }
    }
}
