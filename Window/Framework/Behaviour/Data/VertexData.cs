using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public struct VertexData
    {
        public VertexObject ObjectData;
        public BufferUsageHint UsageHint;
        public PrimitiveType Primitive;
        public ShadingModel Shading;
    }
}
