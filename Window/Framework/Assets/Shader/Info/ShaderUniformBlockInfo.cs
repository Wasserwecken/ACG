using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public struct ShaderUniformBlockInfo
    {
        public int Layout { get; set; }
        public string Name { get; set; }
        public BufferTarget Target { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ShaderUniformBlockInfo(int layout, string name, BufferTarget target)
        {
            Layout = layout;
            Target = target;
            Name = name;
        }
    }
}
