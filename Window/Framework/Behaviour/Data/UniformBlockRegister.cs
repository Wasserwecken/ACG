using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Framework
{
    public class UniformBlockRegister
    {
        public ShaderUniformBlock<TimeData> TimeBlock;
        public ShaderUniformBlock<RenderSpaceData> SpaceBlock;
        public int SpaceBlockLayout { get; set; }
    }
}
