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
        public static void Use(LightData lightData, MaterialData materialData)
        {
            //if (materialData.Shader.GetUniform(Definitions.Shader.Uniforms.Light.Ambient.COLOR, out var ambientColorUniform))
            //    GL.Uniform3(ambientColorUniform.Layout, lightData.AmbientColor);


            //if (materialData.Shader.GetUniform(Definitions.Shader.Uniforms.Light.Directional.COUNT, out var directionalCountUnifrom))
            //    GL.Uniform1(directionalCountUnifrom.Layout, lightData.DirectionalCount);

            //if (materialData.Shader.GetUniform(Definitions.Shader.Uniforms.Light.Directional.COLOR, out var directionalColorUnifrom))
            //    GL.Uniform3(directionalColorUnifrom.Layout, lightData.DirectionalColors.Length, lightData.DirectionalColors);

            //if (materialData.Shader.GetUniform(Definitions.Shader.Uniforms.Light.Directional.DIRECTION, out var directionalDirectionUnifrom))
            //    GL.Uniform3(directionalDirectionUnifrom.Layout, lightData.DirectionalDirections.Length, lightData.DirectionalDirections);
        }
    }
}
