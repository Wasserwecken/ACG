using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public static class ShaderProgramFactory
    {
        /// <summary>
        /// 
        /// </summary>
        public static ShaderProgramAsset Create(params ShaderSource[] shaders)
        {
            CreateProgram(shaders, out var handle);
            AnalyseAttributes(handle, out var attributes);
            AnalyseUniforms(handle, out var uniforms);
            AnalyseStorageBlocks(handle, out var storageBlocks);
            AnalyseUniformBlocks(handle, out var uniformBlocks);

            return new ShaderProgramAsset(handle, attributes, uniforms, uniformBlocks, storageBlocks);
        }

        /// <summary>
        /// 
        /// </summary>
        private static void CreateProgram(ShaderSource[] shaders, out int handle)
        {
            handle = GL.CreateProgram();
            foreach (var shader in shaders)
                GL.AttachShader(handle, shader.Handle);

            GL.LinkProgram(handle);
            GL.GetProgramInfoLog(handle, out var log);
            if (log != string.Empty)
                Console.WriteLine($"ShaderProgram: {log}");

            foreach (var shader in shaders)
                GL.DetachShader(handle, shader.Handle);
        }

        /// <summary>
        /// 
        /// </summary>
        private static void AnalyseAttributes(int handle, out ShaderAttributeInfo[] attributes)
        {
            GL.GetProgram(handle, GetProgramParameterName.ActiveAttributes, out int attributeCount);
            attributes = new ShaderAttributeInfo[attributeCount];

            for (int i = 0; i < attributeCount; i++)
            {
                GL.GetActiveAttrib(handle, i, 255, out int length, out int size, out var type, out var name);
                attributes[i] = new ShaderAttributeInfo(type, GL.GetAttribLocation(handle, name), name, length, size);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static void AnalyseUniforms(int handle, out ShaderUniformInfo[] uniforms)
        {
            GL.GetProgram(handle, GetProgramParameterName.ActiveUniforms, out int uniformCount);
            uniforms = new ShaderUniformInfo[uniformCount];

            for (int i = 0; i < uniformCount; i++)
            {
                GL.GetActiveUniform(handle, i, 255, out int length, out int size, out var type, out var name);
                uniforms[i] = new ShaderUniformInfo(type, GL.GetUniformLocation(handle, name), name, length, size);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static void AnalyseStorageBlocks(int handle, out ShaderUniformBlockInfo[] storageBlocks)
        {
            GL.GetProgramInterface(handle, ProgramInterface.ShaderStorageBlock, ProgramInterfaceParameter.ActiveResources, out var storageBlockCount);
            storageBlocks = new ShaderUniformBlockInfo[storageBlockCount];
            for (int i = 0; i < storageBlockCount; i++)
            {
                GL.GetProgramResourceName(handle, ProgramInterface.ShaderStorageBlock, i, 255, out _, out var name);
                var index = GL.GetProgramResourceIndex(handle, ProgramInterface.ShaderStorageBlock, name);
                storageBlocks[i] = new ShaderUniformBlockInfo(index, name);
            }
        }        
        
        /// <summary>
        /// 
        /// </summary>
        private static void AnalyseUniformBlocks(int handle, out ShaderUniformBlockInfo[] uniformBlocks)
        {
            GL.GetProgramInterface(handle, ProgramInterface.UniformBlock, ProgramInterfaceParameter.ActiveResources, out var uniformBlockCount);
            uniformBlocks = new ShaderUniformBlockInfo[uniformBlockCount];
            for (int i = 0; i < uniformBlockCount; i++)
            {
                GL.GetProgramResourceName(handle, ProgramInterface.UniformBlock, i, 255, out _, out var name);
                var index = GL.GetProgramResourceIndex(handle, ProgramInterface.UniformBlock, name);
                uniformBlocks[i] = new ShaderUniformBlockInfo(index, name);
            }
        }
    }
}
