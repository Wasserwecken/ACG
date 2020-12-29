using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Framework
{
    public struct RenderData
    {
        public ShaderUniformBlock<TimeData> TimeBlock;
        public ShaderUniformBlock<SpaceData> SpaceBlock;
        public int SpaceBlockLayout { get; set; }
    }
}
