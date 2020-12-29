using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Framework
{
    public class ShaderSystem
    {
        public static void Use(ShaderProgramAsset shader, ref RenderData renderData)
        {
            GL.UseProgram(shader.Handle);

            if (shader.GetLayout("TimeBlock", out var timeBlockLayout))
                GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, timeBlockLayout, renderData.TimeBlock.Handle);

            if (shader.GetLayout("SpaceBlock", out var spaceBlockLayout))
                renderData.SpaceBlockLayout = spaceBlockLayout;
        }
    }
}
