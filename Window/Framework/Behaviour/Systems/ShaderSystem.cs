using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Framework
{
    public class ShaderSystem
    {
        public static void Use(MaterialData materialData, ref RenderData renderData)
        {
            GL.UseProgram(materialData.Shader.Handle);


            if (materialData.Shader.GetUniform(Definitions.Shader.Uniforms.Time.DELTA, out var timeDeltaUniform))
                GL.Uniform1(timeDeltaUniform.Layout, renderData.TimeDelta);

            if (materialData.Shader.GetUniform(Definitions.Shader.Uniforms.Time.TOTAL, out var timeTotalUniform))
                GL.Uniform1(timeTotalUniform.Layout, renderData.TimeTotal);

            if (materialData.Shader.GetUniform(Definitions.Shader.Uniforms.View.POSITION, out var viewPositionUniform))
                GL.Uniform3(viewPositionUniform.Layout, renderData.ViewPosition);


            if (materialData.Shader.GetUniform(Definitions.Shader.Uniforms.Space.PROJECTION, out var projectionSpaceUniform))
                renderData.LocalToProjectionSpaceLayout = projectionSpaceUniform.Layout;

            if (materialData.Shader.GetUniform(Definitions.Shader.Uniforms.Space.VIEW, out var viewSpaceUniform))
                renderData.LocalToViewSpaceLayout = viewSpaceUniform.Layout;
            
            if (materialData.Shader.GetUniform(Definitions.Shader.Uniforms.Space.VIEW_ROTATION, out var viewRotationSpaceUniform))
                renderData.LocalToViewRotationSpaceLayout = viewRotationSpaceUniform.Layout;

            if (materialData.Shader.GetUniform(Definitions.Shader.Uniforms.Space.WORLD, out var worldSpaceUniform))
                renderData.LocalToWorldSpaceLayout = worldSpaceUniform.Layout;
            
            if (materialData.Shader.GetUniform(Definitions.Shader.Uniforms.Space.WORLD_ROTATION, out var worldRotationSpaceUniform))
                renderData.LocalToWorldRotationSpaceLayout = worldRotationSpaceUniform.Layout;
        }
    }
}
