using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public class ShaderProgramAsset
    {
        public int Handle { get; }
        public ShaderAttributeInfo[] Attributes { get; }
        public ShaderUniformInfo[] Uniforms { get; }
        public ShaderUniformBlockInfo[] StorageBlocks { get; }
        public ShaderUniformBlockInfo[] UniformBlocks { get; }

        private readonly Dictionary<string, int> _nameToLayout;

        /// <summary>
        /// 
        /// </summary>
        public ShaderProgramAsset(
            int handle,
            ShaderAttributeInfo[] attributes,
            ShaderUniformInfo[] uniforms,
            ShaderUniformBlockInfo[] uniformBlocks,
            ShaderUniformBlockInfo[] storageBlocks)
        {
            _nameToLayout = new Dictionary<string, int>();
            
            Handle = handle;
            Attributes = attributes;
            Uniforms = uniforms;
            UniformBlocks = uniformBlocks;
            StorageBlocks = storageBlocks;

            foreach (var attribute in attributes)
                _nameToLayout.Add(attribute.Name, attribute.Layout);

            foreach (var uniform in uniforms)
                _nameToLayout.Add(uniform.Name, uniform.Layout);

            foreach (var uniformBlock in uniformBlocks)
                _nameToLayout.Add(uniformBlock.Name, uniformBlock.Layout);

            foreach (var storageBlock in storageBlocks)
                _nameToLayout.Add(storageBlock.Name, storageBlock.Layout);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool GetLayout(string name, out int layout)
        {
            return _nameToLayout.TryGetValue(name, out layout);
        }
    }
}
