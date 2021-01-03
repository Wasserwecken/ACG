using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    [DebuggerDisplay("Layout: {Layout}, Target: {Target}, Name: {Name}")]
    public struct ShaderUniformBlockInfo
    {
        public int Layout { get; set; }
        public BufferTarget Target { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ShaderUniformBlockInfo(int layout, BufferTarget target, string name)
        {
            Layout = layout;
            Target = target;
            Name = name;
        }
    }
}
