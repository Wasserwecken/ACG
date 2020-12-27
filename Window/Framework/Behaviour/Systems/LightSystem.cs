using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public class LightSystem
    {
        /// <summary>
        /// 
        /// </summary>
        public static void Use(AmbientLightData ambientLightData, MaterialData materialData)
        {
            if (materialData.Shader.GetUniform(Definitions.Shader.Uniforms.Light.Ambient.COLOR, out var colorUniform))
                GL.Uniform3(colorUniform.Layout, ambientLightData.Color);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Use(DirectionalLightData directionalLightData, MaterialData materialData)
        {
            if (materialData.Shader.GetUniform(Definitions.Shader.Uniforms.Light.Directional.COLOR, out var colorUniform))
                GL.Uniform3(colorUniform.Layout, directionalLightData.Color);

            if (materialData.Shader.GetUniform(Definitions.Shader.Uniforms.Light.Directional.DIRECTION, out var directionUniform))
                GL.Uniform3(directionUniform.Layout, directionalLightData.Direction);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        //public static void Use(DirectionalLightData[] directionalLightData, MaterialData materialData)
        //{


        //    if (materialData.Shader.GetUniform(Definitions.Shader.Uniforms.Light.Directional.COLOR, out var colorUniform))
        //        GL.Uniform3(colorUniform.Layout, directionalLightData.Color);

        //    if (materialData.Shader.GetUniform(Definitions.Shader.Uniforms.Light.Directional.DIRECTION, out var directionUniform))
        //        GL.Uniform3(directionUniform.Layout, directionalLightData.Direction);
        //}
    }
}
