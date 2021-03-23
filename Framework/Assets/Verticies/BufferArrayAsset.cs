using System.Collections.Generic;
using System.Linq;
using Framework.Assets.Verticies.Attributes;
using OpenTK.Graphics.OpenGL;

namespace Framework.Assets.Verticies
{
    public class BufferArrayAsset : BufferBaseAsset
    {
        public override int ElementSize => Attributes.Sum(a => a.ElementSize);
        public override int ElementCount => Attributes[0].ElementCount;
        public List<IVertexAttribute> Attributes { get; }


        /// <summary>
        /// 
        /// </summary>
        public BufferArrayAsset(BufferUsageHint usageHint)
            : this(usageHint, new IVertexAttribute[0]) { }

        /// <summary>
        /// 
        /// </summary>
        public BufferArrayAsset(BufferUsageHint usageHint, IEnumerable<IVertexAttribute> attributes)
            : base(usageHint, BufferTarget.ArrayBuffer, "VertexArray")
        {
            Attributes = new List<IVertexAttribute>(attributes);
        }
    }
}
