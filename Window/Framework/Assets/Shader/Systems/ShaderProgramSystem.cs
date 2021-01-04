using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public static class ShaderProgramSystem
    {
        /// <summary>
        /// 
        /// </summary>
        public static void CreateAndAnalyse(ShaderProgramAsset program, params ShaderSourceAsset[] sources)
        {
            CreateProgram(program, sources);

            AnalyseAttributes(program);
            AnalyseUniforms(program);
            AnalyseBlocks(program);

            UpdateIdentifierToLayout(program);
        }

        /// <summary>
        /// 
        /// </summary>
        private static void CreateProgram(ShaderProgramAsset program, ShaderSourceAsset[] sources)
        {
            program.Handle = GL.CreateProgram();
            foreach (var shader in sources)
                GL.AttachShader(program.Handle, shader.Handle);

            GL.LinkProgram(program.Handle);
            GL.GetProgramInfoLog(program.Handle, out var log);
            if (log != string.Empty)
                Console.WriteLine($"ShaderProgram: {log}");
        }

        /// <summary>
        /// 
        /// </summary>
        private static void AnalyseAttributes(ShaderProgramAsset program)
        {
            GL.GetProgram(program.Handle, GetProgramParameterName.ActiveAttributes, out int attributeCount);
            program.Attributes = new ShaderAttributeInfo[attributeCount];

            for (int i = 0; i < attributeCount; i++)
            {
                GL.GetActiveAttrib(program.Handle, i, 255, out _, out int size, out var type, out var name);
                program.Attributes[i] = new ShaderAttributeInfo(GL.GetAttribLocation(program.Handle, name), type, name, size);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static void AnalyseUniforms(ShaderProgramAsset program)
        {
            GL.GetProgramInterface(program.Handle, ProgramInterface.Uniform, ProgramInterfaceParameter.ActiveResources, out var uniformCount);
            var validUniforms = new List<ShaderUniformInfo>();

            for (int i = 0; i < uniformCount; i++)
            {
                GL.GetActiveUniform(program.Handle, i, 255, out _, out int size, out var type, out var name);
                var layout = GL.GetUniformLocation(program.Handle, name);

                if (layout > -1)
                    validUniforms.Add(new ShaderUniformInfo(layout, type, name, size));
            }

            program.Uniforms = validUniforms.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        private static void AnalyseBlocks(ShaderProgramAsset program)
        {
            GL.GetProgramInterface(program.Handle, ProgramInterface.ShaderStorageBlock, ProgramInterfaceParameter.ActiveResources, out var storageBlockCount);
            GL.GetProgramInterface(program.Handle, ProgramInterface.UniformBlock, ProgramInterfaceParameter.ActiveResources, out var uniformBlockCount);
            program.Blocks = new ShaderUniformBlockInfo[storageBlockCount + uniformBlockCount];

            var blockId = 0;
            for (int storageId = 0; storageId < storageBlockCount; storageId++, blockId++)
            {
                GL.GetProgramResourceName(program.Handle, ProgramInterface.ShaderStorageBlock, storageId, 255, out _, out var name);
                var layout = GL.GetProgramResourceIndex(program.Handle, ProgramInterface.ShaderStorageBlock, name);
                GL.ShaderStorageBlockBinding(program.Handle, layout, layout);
                program.Blocks[blockId] = new ShaderUniformBlockInfo(layout, BufferTarget.ShaderStorageBuffer, name);
            }

            for (int uniformId = 0; uniformId < uniformBlockCount; uniformId++, blockId++)
            {
                GL.GetProgramResourceName(program.Handle, ProgramInterface.UniformBlock, uniformId, 255, out _, out var name);
                var layout = GL.GetProgramResourceIndex(program.Handle, ProgramInterface.UniformBlock, name);
                program.Blocks[blockId] = new ShaderUniformBlockInfo(layout, BufferTarget.UniformBuffer, name);
            }
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
