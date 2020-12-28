using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public struct ShaderUniformInfo
    {
        public ActiveUniformType Type { get; private set; }
        public int Layout { get; private set; }
        public string Name { get; private set; }
        public int Length { get; private set; }
        public int Size { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ShaderUniformInfo(ActiveUniformType type, int layout, string name, int length, int size)
        {
            Type = type;
            Layout = layout;
            Name = name;
            Length = length;
            Size = size;
        }
    }
}
