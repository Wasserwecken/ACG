using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Framework
{
    public class ShaderBlockRegister
    {
        public readonly List<IShaderBlock> Blocks;

        /// <summary>
        /// 
        /// </summary>
        public ShaderBlockRegister(IShaderBlock[] initialBlocks)
        {
            Blocks = new List<IShaderBlock>(initialBlocks);
        }
    }
}
