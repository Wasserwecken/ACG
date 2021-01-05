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
        public ArrayBufferAsset(VertexAttributeAsset[] attributes, BufferUsageHint usageHint)
            : base("VertexArray", attributes.Sum(a => a.ElementSize), BufferTarget.ArrayBuffer, usageHint)
        {
            Attributes = attributes;
        }
    }
}
