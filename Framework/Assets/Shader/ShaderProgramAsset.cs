using System;
using System.Collections.Generic;
using System.Diagnostics;
using ACG.Framework.Assets;
using Framework.Assets.Shader.Info;
using OpenTK.Graphics.OpenGL;

namespace Framework.Assets.Shader
{
    [DebuggerDisplay("Handle: {Handle}," +
        " Name: {Name}," +
        " Attributes: {AttributeInfos.Length}," +
        " Uniforms: {UniformInfos.Length}," +
        " Blocks: {BlockInfos.Length}," +
        " Bindings: {BlockBindings.Count}")]
    public class ShaderProgramAsset
    {
        [DebuggerDisplay("Target: {Target}, Layout: {Layout}, Handle: {Handle}")]
        public struct BlockBinding
        {
            public int Layout;
            public int Handle;
            public BufferRangeTarget Target;

            public BlockBinding(int layout, ShaderBlockBase block)
            {
                Layout = layout;
                Handle = block.Handle;
                Target = block.Target;
            }
        }

        public int Handle;
        public string Name;
        public readonly ShaderAttributeInfo[] AttributeInfos;
        public readonly ShaderUniformInfo[] UniformInfos;
        public readonly ShaderBlockInfo[] BlockInfos;
        public readonly Dictionary<string, int> IdentifierToLayout;
        public readonly Dictionary<string, BlockBinding> BlockBindings;

        /// <summary>
        /// 
        /// </summary>
        public ShaderProgramAsset(string name, params ShaderSourceAsset[] sources)
        {
            Name = name;
            Handle = CreateProgramHandle(sources);
            IdentifierToLayout = new Dictionary<string, int>();

            AttributeInfos = GetAttributesInfos();
            UniformInfos = GetUniformInfos();
            BlockInfos = GetBlockInfos();
            BlockBindings = new Dictionary<string, BlockBinding>();

            AssetRegister.Shaders.Add(this);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetBlockBinding(ShaderBlockBase block)
        {
            if (IdentifierToLayout.TryGetValue(block.Name, out var layout))
                if (BlockBindings.ContainsKey(block.Name))
                    BlockBindings[block.Name] = new BlockBinding(layout, block);
                else
                    BlockBindings.Add(block.Name, new BlockBinding(layout, block));
        }

        /// <summary>
        /// 
        /// </summary>
        private int CreateProgramHandle(ShaderSourceAsset[] sources)
        {
            var handle = GL.CreateProgram();
            foreach (var shader in sources)
                GL.AttachShader(handle, shader.Handle);

            GL.LinkProgram(handle);
            GL.GetProgramInfoLog(handle, out var log);
            if (log != string.Empty)
                Console.WriteLine($"ShaderProgram '{Name}': {log}");

            return handle;
        }

        /// <summary>
        /// 
        /// </summary>
        private ShaderAttributeInfo[] GetAttributesInfos()
        {
            GL.GetProgram(Handle, GetProgramParameterName.ActiveAttributes, out int attributeCount);
            var infos = new ShaderAttributeInfo[attributeCount];

            for (int i = 0; i < attributeCount; i++)
            {
                GL.GetActiveAttrib(Handle, i, 255, out _, out int size, out var type, out var name);
                infos[i] = new ShaderAttributeInfo(GL.GetAttribLocation(Handle, name), type, name, size);
            }

            foreach (var attribute in infos)
                IdentifierToLayout.Add(attribute.Name, attribute.Layout);
            
            return infos;
        }

        /// <summary>
        /// 
        /// </summary>
        private ShaderUniformInfo[] GetUniformInfos()
        {
            GL.GetProgramInterface(Handle, ProgramInterface.Uniform, ProgramInterfaceParameter.ActiveResources, out var uniformCount);
            var validUniforms = new List<ShaderUniformInfo>();

            for (int i = 0; i < uniformCount; i++)
            {
                GL.GetActiveUniform(Handle, i, 255, out _, out int size, out var type, out var name);
                var layout = GL.GetUniformLocation(Handle, name);

                if (layout > -1)
                    validUniforms.Add(new ShaderUniformInfo(layout, type, name, size));
            }

            foreach (var uniform in validUniforms)
                IdentifierToLayout.Add(uniform.Name, uniform.Layout);

            return validUniforms.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        private ShaderBlockInfo[] GetBlockInfos()
        {
            GL.GetProgramInterface(Handle, ProgramInterface.ShaderStorageBlock, ProgramInterfaceParameter.ActiveResources, out var storageBlockCount);
            GL.GetProgramInterface(Handle, ProgramInterface.UniformBlock, ProgramInterfaceParameter.ActiveResources, out var uniformBlockCount);
            var infos = new ShaderBlockInfo[storageBlockCount + uniformBlockCount];

            var blockId = 0;
            for (int storageId = 0; storageId < storageBlockCount; storageId++, blockId++)
            {
                GL.GetProgramResourceName(Handle, ProgramInterface.ShaderStorageBlock, storageId, 255, out _, out var name);
                var layout = GL.GetProgramResourceIndex(Handle, ProgramInterface.ShaderStorageBlock, name);
                GL.ShaderStorageBlockBinding(Handle, layout, layout);
                infos[blockId] = new ShaderBlockInfo(layout, BufferTarget.ShaderStorageBuffer, name);
            }

            for (int uniformId = 0; uniformId < uniformBlockCount; uniformId++, blockId++)
            {
                GL.GetProgramResourceName(Handle, ProgramInterface.UniformBlock, uniformId, 255, out _, out var name);
                var layout = GL.GetProgramResourceIndex(Handle, ProgramInterface.UniformBlock, name);
                GL.UniformBlockBinding(Handle, layout, layout);
                infos[blockId] = new ShaderBlockInfo(layout, BufferTarget.UniformBuffer, name);
            }

            foreach (var uniformBlock in infos)
                IdentifierToLayout.Add(uniformBlock.Name, uniformBlock.Layout);

            return infos;
        }
    }
}
