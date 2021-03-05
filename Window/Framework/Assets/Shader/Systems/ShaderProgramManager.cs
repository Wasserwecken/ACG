using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public static class ShaderProgramManager
    {
        /// <summary>
        /// 
        /// </summary>
        public static ShaderProgramAsset Create(string name, params ShaderSourceAsset[] sources)
        {
            var handle = CreateProgramHandle(sources);
            var program = new ShaderProgramAsset()
            {
                Name = name,
                Handle = handle,
                Attributes = GetAttributesInfos(handle),
                Uniforms = GetUniformInfos(handle),
                Blocks = GetBlockInfos(handle),
            };

            UpdateIdentifierToLayout(program);
            return program;
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
        private static ShaderAttributeInfo[] GetAttributesInfos(int programHandle)
        {
            GL.GetProgram(programHandle, GetProgramParameterName.ActiveAttributes, out int attributeCount);
            var infos = new ShaderAttributeInfo[attributeCount];

            for (int i = 0; i < attributeCount; i++)
            {
                GL.GetActiveAttrib(programHandle, i, 255, out _, out int size, out var type, out var name);
                infos[i] = new ShaderAttributeInfo(GL.GetAttribLocation(programHandle, name), type, name, size);
            }

            return infos;
        }

        /// <summary>
        /// 
        /// </summary>
        private static ShaderUniformInfo[] GetUniformInfos(int programHandle)
        {
            GL.GetProgramInterface(programHandle, ProgramInterface.Uniform, ProgramInterfaceParameter.ActiveResources, out var uniformCount);
            var validUniforms = new List<ShaderUniformInfo>();

            for (int i = 0; i < uniformCount; i++)
            {
                GL.GetActiveUniform(programHandle, i, 255, out _, out int size, out var type, out var name);
                var layout = GL.GetUniformLocation(programHandle, name);

                if (layout > -1)
                    validUniforms.Add(new ShaderUniformInfo(layout, type, name, size));
            }

            return validUniforms.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        private static ShaderBlockInfo[] GetBlockInfos(int programHandle)
        {
            GL.GetProgramInterface(programHandle, ProgramInterface.ShaderStorageBlock, ProgramInterfaceParameter.ActiveResources, out var storageBlockCount);
            GL.GetProgramInterface(programHandle, ProgramInterface.UniformBlock, ProgramInterfaceParameter.ActiveResources, out var uniformBlockCount);
            var infos = new ShaderBlockInfo[storageBlockCount + uniformBlockCount];

            var blockId = 0;
            for (int storageId = 0; storageId < storageBlockCount; storageId++, blockId++)
            {
                GL.GetProgramResourceName(programHandle, ProgramInterface.ShaderStorageBlock, storageId, 255, out _, out var name);
                var layout = GL.GetProgramResourceIndex(programHandle, ProgramInterface.ShaderStorageBlock, name);
                GL.ShaderStorageBlockBinding(programHandle, layout, layout);
                infos[blockId] = new ShaderBlockInfo(layout, BufferTarget.ShaderStorageBuffer, name);
            }

            for (int uniformId = 0; uniformId < uniformBlockCount; uniformId++, blockId++)
            {
                GL.GetProgramResourceName(programHandle, ProgramInterface.UniformBlock, uniformId, 255, out _, out var name);
                var layout = GL.GetProgramResourceIndex(programHandle, ProgramInterface.UniformBlock, name);
                GL.UniformBlockBinding(programHandle, layout, layout);
                infos[blockId] = new ShaderBlockInfo(layout, BufferTarget.UniformBuffer, name);
            }

            return infos;
        }

        /// <summary>
        /// 
        /// </summary>
        private static void UpdateIdentifierToLayout(ShaderProgramAsset program)
        {
            program.IdentifierToLayout = new Dictionary<string, int>();

            foreach (var attribute in program.Attributes)
                program.IdentifierToLayout.Add(attribute.Name, attribute.Layout);

            foreach (var uniform in program.Uniforms)
                program.IdentifierToLayout.Add(uniform.Name, uniform.Layout);

            foreach (var uniformBlock in program.Blocks)
                program.IdentifierToLayout.Add(uniformBlock.Name, uniformBlock.Layout);
        }
    }
}
