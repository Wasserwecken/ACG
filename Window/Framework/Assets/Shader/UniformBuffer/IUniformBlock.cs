using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public interface IUniformBlock
    {
        public int Handle { get; }
        public string Name { get; }
        public BufferRangeTarget Target { get; }
    }
}
