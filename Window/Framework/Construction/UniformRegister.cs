using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Framework
{
    public class UniformRegister
    {
        public readonly List<IUniformBlock> Blocks;

        /// <summary>
        /// 
        /// </summary>
        public UniformRegister(IUniformBlock[] initialBlocks)
        {
            Blocks = new List<IUniformBlock>(initialBlocks);
        }
    }
}
