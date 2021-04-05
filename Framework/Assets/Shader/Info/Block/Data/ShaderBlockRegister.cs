using System.Collections.Generic;

namespace Framework.Assets.Shader.Block.Data
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
    }
}
