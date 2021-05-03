using System;
using System.Collections.Generic;

namespace Framework.Assets.Shader.Block.Data
{
    public static class ShaderBlockRegister
    {
        public static readonly Dictionary<string, IShaderBlock> Blocks;

        /// <summary>
        /// 
        /// </summary>
        static ShaderBlockRegister()
        {
            Blocks = new Dictionary<string, IShaderBlock>();
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Add(IShaderBlock block)
        {
            Blocks.Remove(block.Name);
            Blocks.Add(block.Name, block);
        }

        /// <summary>
        /// 
        /// </summary>
        public static IShaderBlock Get<TBlockData>() where TBlockData : struct
        {
            return Blocks[typeof(TBlockData).Name];
        }
    }
}
