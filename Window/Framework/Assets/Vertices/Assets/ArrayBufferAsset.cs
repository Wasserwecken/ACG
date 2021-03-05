using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public class ArrayBufferAsset : BufferBase
    {
        public VertexAttributeAsset[] Attributes { get; }

        /// <summary>
        /// 
        /// </summary>
        public ArrayBufferAsset(BufferUsageHint usageHint)
            : this(usageHint, new VertexAttributeAsset[0]) { }

        /// <summary>
        /// 
        /// </summary>
        public ArrayBufferAsset(BufferUsageHint usageHint, VertexAttributeAsset[] attributes)
            : base(usageHint, BufferTarget.ArrayBuffer, "VertexArray", attributes.Sum(a => a.ElementSize))
        {
            Attributes = attributes;
        }
    }
}
