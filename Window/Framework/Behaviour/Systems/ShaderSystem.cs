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

            if (shader.GetLayout(Definitions.Shader.Uniforms.Time.DELTA, out var timeDeltaLayout))
                GL.Uniform1(timeDeltaLayout, renderData.TimeDelta);

            if (shader.GetLayout(Definitions.Shader.Uniforms.Time.TOTAL, out var timeTotalLayout))
                GL.Uniform1(timeTotalLayout, renderData.TimeTotal);

            if (shader.GetLayout(Definitions.Shader.Uniforms.View.POSITION, out var viewPositionLayout))
                GL.Uniform3(viewPositionLayout, renderData.ViewPosition);


            if (shader.GetLayout(Definitions.Shader.Uniforms.Space.PROJECTION, out var projectionSpaceLayout))
                renderData.LocalToProjectionSpaceLayout = projectionSpaceLayout;

            if (shader.GetLayout(Definitions.Shader.Uniforms.Space.VIEW, out var viewSpaceLayout))
                renderData.LocalToViewSpaceLayout = viewSpaceLayout;
            
            if (shader.GetLayout(Definitions.Shader.Uniforms.Space.VIEW_ROTATION, out var viewRotationSpaceLayout))
                renderData.LocalToViewRotationSpaceLayout = viewRotationSpaceLayout;

            if (shader.GetLayout(Definitions.Shader.Uniforms.Space.WORLD, out var worldSpaceLayout))
                renderData.LocalToWorldSpaceLayout = worldSpaceLayout;
            
            if (shader.GetLayout(Definitions.Shader.Uniforms.Space.WORLD_ROTATION, out var worldRotationSpaceLayout))
                renderData.LocalToWorldRotationSpaceLayout = worldRotationSpaceLayout;
        }
    }
}
