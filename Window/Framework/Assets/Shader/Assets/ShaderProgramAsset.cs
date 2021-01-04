using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Framework
{
    [DebuggerDisplay("Handle: {Handle}, Name: {Name}, Attributes: {Attributes.Length}, Uniforms: {Uniforms.Length}, Blocks: {Blocks.Length}")]
    public class ShaderProgramAsset
    {
        public int Handle { get; set; }
        public string Name { get; }
        public ShaderAttributeInfo[] Attributes { get; set; }
        public ShaderUniformInfo[] Uniforms { get; set; }
        public ShaderUniformBlockInfo[] Blocks { get; set; }
        public Dictionary<string, int> IdentifierToLayout { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ShaderProgramAsset(string name)
        {
            Name = name;
        }
    }
}
