using System;
using System.Collections.Generic;
using System.Diagnostics;
using Framework.Assets.Shader.Block;
using Framework.Assets.Shader.Info;
using OpenTK.Graphics.OpenGL;

namespace Framework.Assets.Shader
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
        public ShaderProgramAsset(string name, params ShaderSourceAsset[] sources)
        {
            IdentifierToLayout = new Dictionary<string, int>();
            Handle = CreateProgramHandle(sources);
            Name = name;

            GetAttributesInfos();
            GetUniformInfos();
            GetBlockInfos();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Use()
        {
            GL.UseProgram(Handle);
            foreach (var block in ShaderBlockRegister.Blocks)
                if (IdentifierToLayout.TryGetValue(block.Name, out var blockLayout))
                    GL.BindBufferBase(block.Target, blockLayout, block.Handle);
        }

        /// <summary>
        /// 
        /// </summary>
        private static int CreateProgramHandle(ShaderSourceAsset[] sources)
        {
            var handle = GL.CreateProgram();
            foreach (var shader in sources)
                GL.AttachShader(handle, shader.Handle);

            GL.LinkProgram(handle);
            GL.GetProgramInfoLog(handle, out var log);
            if (log != string.Empty)
                Console.WriteLine($"ShaderProgram: {log}");

            return handle;
        }

        /// <summary>
        /// 
        /// </summary>
        private void GetAttributesInfos()
        {
            GL.GetProgram(Handle, GetProgramParameterName.ActiveAttributes, out int attributeCount);
            var infos = new ShaderAttributeInfo[attributeCount];

            for (int i = 0; i < attributeCount; i++)
            {
                GL.GetActiveAttrib(Handle, i, 255, out _, out int size, out var type, out var name);
                infos[i] = new ShaderAttributeInfo(GL.GetAttribLocation(Handle, name), type, name, size);
            }

            Attributes = infos;
            foreach (var attribute in Attributes)
                IdentifierToLayout.Add(attribute.Name, attribute.Layout);
        }

        /// <summary>
        /// 
        /// </summary>
        private void GetUniformInfos()
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

            Uniforms = validUniforms.ToArray();
            foreach (var uniform in Uniforms)
                IdentifierToLayout.Add(uniform.Name, uniform.Layout);
        }

        /// <summary>
        /// 
        /// </summary>
        private void GetBlockInfos()
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

            Blocks = infos;
            foreach (var uniformBlock in Blocks)
                IdentifierToLayout.Add(uniformBlock.Name, uniformBlock.Layout);
        }
    }
}
