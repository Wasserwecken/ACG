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

            return new ShaderProgramAsset(handle, attributes, uniforms, storageBlocks);
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
        private static void AnalyseStorageBlocks(int handle, out ShaderStorageBlockInfo[] storageBlocks)
        {
            GL.GetProgramInterface(handle, ProgramInterface.ShaderStorageBlock, ProgramInterfaceParameter.ActiveResources, out var storageBlockCount);
            storageBlocks = new ShaderStorageBlockInfo[storageBlockCount];
            for (int i = 0; i < storageBlockCount; i++)
            {
                GL.GetProgramResourceName(handle, ProgramInterface.ShaderStorageBlock, i, 255, out var nameLength, out var name);
                var location = GL.GetProgramResourceIndex(handle, ProgramInterface.ShaderStorageBlock, name);
                storageBlocks[i] = new ShaderStorageBlockInfo(location, name);
            }
        }
    }
}
