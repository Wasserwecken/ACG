using System.Diagnostics;
using System.Linq;
using OpenTK.Graphics.OpenGL;

namespace Framework.Assets.Verticies
{
    [DebuggerDisplay("Name: {Name}, Handle: {Handle}, ElementSize: {ElementSize}, UsageHint: {UsageHint}, ElementCount: {Attributes?[0]?.ElementCount}")]
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
        }
    }
}
