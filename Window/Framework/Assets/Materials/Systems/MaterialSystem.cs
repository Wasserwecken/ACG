using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Framework
{
    public class MaterialSystem
    {
        public static void Use(MaterialAsset material)
        {
            GL.ShadeModel(material.Model);
            GL.FrontFace(material.FaceDirection);

            if (material.IsDepthTesting)
                GL.Enable(EnableCap.DepthTest);

            if (material.IsCulling)
            {
                GL.Enable(EnableCap.CullFace);
                GL.CullFace(material.CullingMode);
            }
            else
                GL.Disable(EnableCap.CullFace);

            if (material.IsTransparent)
            {
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(material.SourceBlend, material.DestinationBlend);
            }
            else
                GL.Disable(EnableCap.Blend);


            foreach (var uniform in material.UniformTextures)
                UpdateTextureUniform(uniform.Key, uniform.Value);

            foreach (var uniform in material.UniformFloats)
                GL.Uniform1(uniform.Key, uniform.Value);

            foreach (var uniform in material.UniformVec3s)
                GL.Uniform3(uniform.Key, uniform.Value);

            foreach (var uniform in material.UniformMat4s)
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
