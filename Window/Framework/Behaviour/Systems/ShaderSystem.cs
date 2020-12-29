using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Framework
{
    public class ShaderSystem
    {
        public static void Use(ShaderProgramAsset shader, UniformRegister uniformRegister)
        {
            GL.UseProgram(shader.Handle);

            foreach (var block in uniformRegister.StorageBlocks)
                if (shader.GetLayout(block.Name, out var blockLayout))
                    GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, blockLayout, block.Handle);

            foreach (var block in uniformRegister.UniformBlocks)
                if (shader.GetLayout(block.Name, out var blockLayout))
                    GL.BindBufferBase(BufferRangeTarget.UniformBuffer, blockLayout, block.Handle);
        }
    }
}
