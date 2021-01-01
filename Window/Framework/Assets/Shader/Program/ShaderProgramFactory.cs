﻿using System;
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
            AnalyseBlocks(handle, out var blocks);

            return new ShaderProgramAsset(handle, attributes, uniforms, blocks);
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
        private static void AnalyseBlocks(int handle, out ShaderUniformBlockInfo[] blocks)
        {
            GL.GetProgramInterface(handle, ProgramInterface.ShaderStorageBlock, ProgramInterfaceParameter.ActiveResources, out var storageBlockCount);
            GL.GetProgramInterface(handle, ProgramInterface.UniformBlock, ProgramInterfaceParameter.ActiveResources, out var uniformBlockCount);
            blocks = new ShaderUniformBlockInfo[storageBlockCount + uniformBlockCount];

            var blockId = 0;
            for (int storageId = 0; storageId < storageBlockCount; storageId++, blockId++)
            {
                GL.GetProgramResourceName(handle, ProgramInterface.ShaderStorageBlock, storageId, 255, out _, out var name);
                var layout = GL.GetProgramResourceIndex(handle, ProgramInterface.ShaderStorageBlock, name);
                GL.ShaderStorageBlockBinding(handle, layout, layout);
                blocks[blockId] = new ShaderUniformBlockInfo(layout, name, BufferTarget.ShaderStorageBuffer);
            }

            for (int uniformId = 0; uniformId < uniformBlockCount; uniformId++, blockId++)
            {
                GL.GetProgramResourceName(handle, ProgramInterface.UniformBlock, uniformId, 255, out _, out var name);
                var layout = GL.GetProgramResourceIndex(handle, ProgramInterface.UniformBlock, name);
                blocks[blockId] = new ShaderUniformBlockInfo(layout, name, BufferTarget.UniformBuffer);
            }
        }
    }
}
