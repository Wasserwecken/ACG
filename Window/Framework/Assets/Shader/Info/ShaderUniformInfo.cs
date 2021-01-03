using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    [DebuggerDisplay("Layout: {Layout}, Type: {Type}, Name: {Name}")]
    public struct ShaderUniformInfo
    {
        public int Layout { get; private set; }
        public ActiveUniformType Type { get; private set; }
        public string Name { get; private set; }
        public int Size { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ShaderUniformInfo(int layout, ActiveUniformType type, string name, int size)
        {
            Layout = layout;
            Type = type;
            Name = name;
            Size = size;
        }
    }
}
