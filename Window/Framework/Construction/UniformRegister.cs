using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Framework
{
    public class UniformRegister
    {
        public int SpaceBlockLayout { get; set; }
        public readonly List<IUniformBlock> StorageBlocks;
        public readonly List<IUniformBlock> UniformBlocks;

        /// <summary>
        /// 
        /// </summary>
        public UniformRegister(IUniformBlock[] storageBlocks, IUniformBlock[] uniformBlock)
        {
            StorageBlocks = new List<IUniformBlock>(storageBlocks);
            UniformBlocks = new List<IUniformBlock>(uniformBlock);
        }
    }
}
