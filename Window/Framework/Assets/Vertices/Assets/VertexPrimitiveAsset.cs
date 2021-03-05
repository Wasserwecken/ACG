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
        public VertexPrimitiveAsset(VertexAttributeAsset[] attributes, uint[] indicies, BufferUsageHint usageHint)
            : this(new ArrayBufferAsset(usageHint, attributes), new IndicieBufferAsset(usageHint, indicies)) { }

        /// <summary>
        /// 
        /// </summary>
        public VertexPrimitiveAsset()
            : this(new ArrayBufferAsset(BufferUsageHint.StaticDraw), new IndicieBufferAsset(BufferUsageHint.StaticDraw)) { }

        /// <summary>
        /// 
        /// </summary>
        public VertexPrimitiveAsset(ArrayBufferAsset arrayBuffer, IndicieBufferAsset indicieBuffer)
        {
            ArrayBuffer = arrayBuffer;
            IndicieBuffer = indicieBuffer;

            Mode = PolygonMode.Fill;
            Type = PrimitiveType.Triangles;
        }
    }
}
