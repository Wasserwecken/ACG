using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public struct VertexData
    {
        public static VertexData Default => new VertexData()
        {
            Shading = ShadingModel.Smooth,
            Primitive = PrimitiveType.Triangles,
            UsageHint = BufferUsageHint.StaticDraw
        };

        public VertexObject ObjectData { get; set; }
        public BufferUsageHint UsageHint { get; set; }
        public PrimitiveType Primitive { get; set; }
        public ShadingModel Shading { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public VertexData(VertexObject objectData)
        {
            this = Default;
            ObjectData = objectData;
        }
    }
}
