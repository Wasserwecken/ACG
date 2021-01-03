using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    [DebuggerDisplay("Type: {Type}, Mode: {Mode}")]
    public class VertexPrimitiveAsset
    {
        public int Handle { get; set; }
        public ArrayBufferAsset ArrayBuffer { get; }
        public IndicieBufferAsset IndicieBuffer { get; }
        public PolygonMode Mode { get; set; }
        public PrimitiveType Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public VertexPrimitiveAsset(ArrayBufferAsset arrayBuffer)
            : this(arrayBuffer, null) { }

        /// <summary>
        /// 
        /// </summary>
        public VertexPrimitiveAsset(ArrayBufferAsset arrayBuffer, IndicieBufferAsset indicieBuffer)
        {
            ArrayBuffer = arrayBuffer;
            IndicieBuffer = indicieBuffer;
        }
    }
}
