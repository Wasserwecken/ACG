using System.Collections.Generic;

namespace Framework.Assets.Shader.Block
{
    public static class ShaderBlockRegister
    {
        public static List<IShaderBlock> Blocks { get; }

        /// <summary>
        /// 
        /// </summary>
        static ShaderBlockRegister()
        {
            Blocks = new List<IShaderBlock>();
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Add(IShaderBlock block)
        {
            Blocks.Add(block);
        }
    }
}
