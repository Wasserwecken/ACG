using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Framework
{
    [DebuggerDisplay("Handle: {Handle}, Name: {Name}, Attributes: {Attributes.Length}, Uniforms: {Uniforms.Length}, Blocks: {Blocks.Length}")]
    public class ShaderProgramAsset
    {
        public int Handle { get; }
        public string Name { get; }
        public ShaderAttributeInfo[] Attributes { get; }
        public ShaderUniformInfo[] Uniforms { get; }
        public ShaderUniformBlockInfo[] Blocks { get; }
        public Dictionary<string, int> IdentifierToLayout { get; }

        /// <summary>
        /// 
        /// </summary>
        public ShaderProgramAsset(
            string name,
            int handle,
            ShaderAttributeInfo[] attributes,
            ShaderUniformInfo[] uniforms,
            ShaderUniformBlockInfo[] blocks)
        {
            IdentifierToLayout = new Dictionary<string, int>();

            Name = name;
            Handle = handle;
            Attributes = attributes;
            Uniforms = uniforms;
            Blocks = blocks;

            foreach (var attribute in attributes)
                IdentifierToLayout.Add(attribute.Name, attribute.Layout);

            foreach (var uniform in uniforms)
                IdentifierToLayout.Add(uniform.Name, uniform.Layout);

            foreach (var uniformBlock in Blocks)
                IdentifierToLayout.Add(uniformBlock.Name, uniformBlock.Layout);
        }
    }
}
