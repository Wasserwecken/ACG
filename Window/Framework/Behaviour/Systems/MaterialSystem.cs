using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Framework
{
    public class MaterialSystem
    {
        public static void Use(MaterialData materialData)
        {
            GL.FrontFace(materialData.FaceDirection);

            if (materialData.IsDepthTesting)
                GL.Enable(EnableCap.DepthTest);

            if (materialData.IsCulling)
            {
                GL.Enable(EnableCap.CullFace);
                GL.CullFace(materialData.CullingMode);
            }

            if (materialData.IsTransparent)
            {
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(materialData.SourceBlend, materialData.DestinationBlend);
            }
            else
                GL.Disable(EnableCap.Blend);


            foreach (var uniform in materialData.UniformTextures)
                UpdateTextureUniform(uniform.Key, uniform.Value);

            foreach (var uniform in materialData.UniformFloats)
                GL.Uniform1(uniform.Key, uniform.Value);

            foreach (var uniform in materialData.UniformVec3s)
                GL.Uniform3(uniform.Key, uniform.Value);

            foreach (var uniform in materialData.UniformMat4s)
            {
                var matrix = uniform.Value;
                GL.UniformMatrix4(uniform.Key, false, ref matrix);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static void UpdateTextureUniform(int layout, Texture texture)
        {
            GL.Uniform1(layout, layout);
            GL.ActiveTexture(TextureUnit.Texture0 + layout);
            GL.BindTexture(TextureTarget.Texture2D, texture.Handle);
        }
    }
}
