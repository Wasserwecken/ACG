using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Framework
{
    [DebuggerDisplay("Handle: {Handle}, Name: {Name}, Attributes: {Attributes.Length}, Uniforms: {Uniforms.Length}, Blocks: {Blocks.Length}")]
    public class ShaderProgramAsset
    {
        public int Handle;
        public string Name;
        public ShaderAttributeInfo[] Attributes;
        public ShaderUniformInfo[] Uniforms;
        public ShaderBlockInfo[] Blocks;
        public Dictionary<string, int> IdentifierToLayout;

        /// <summary>
        /// 
        /// </summary>
        public ShaderProgramAsset()
        {
            Handle = -1;
        }
    }
}
