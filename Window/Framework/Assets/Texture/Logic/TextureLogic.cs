using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public static class TextureLogic
    {
        /// <summary>
        /// 
        /// </summary>
        public static void PushToGPU(Texture2DAsset texture)
        {
            PushToGPUBase(texture, f => GL.TexImage2D(
                f.Target, 0, f.InternalFormat, f.Image.Width, f.Image.Height, 0, f.Format, f.PixelType, f.Image.Pixels));
        }

        /// <summary>
        /// 
        /// </summary>
        public static void PushToGPU(TextureCubeAsset cube)
        {
            PushToGPUBase(cube, f =>
            {
                foreach (var i in f.Images)
                    GL.TexImage2D(f.Target, 0, f.InternalFormat, i.Width, i.Height, 0, f.Format, f.PixelType, i.Pixels);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        private static void PushToGPUBase<TTexture>(TTexture texture, Action<TTexture> glTexImage) where TTexture : TextureBase
        {
            if (texture.Handle <= 0)
                texture.Handle = GL.GenTexture();
            GL.BindTexture(texture.Target, texture.Handle);

            glTexImage(texture);

            GL.TexParameter(texture.Target, TextureParameterName.TextureWrapS, (int)texture.WrapMode);
            GL.TexParameter(texture.Target, TextureParameterName.TextureWrapT, (int)texture.WrapMode);
            GL.TexParameter(texture.Target, TextureParameterName.TextureWrapR, (int)texture.WrapMode);
            GL.TexParameter(texture.Target, TextureParameterName.TextureMinFilter, (int)texture.MinFilter);
            GL.TexParameter(texture.Target, TextureParameterName.TextureMagFilter, (int)texture.MagFilter);

            if (texture.GenerateMipMaps)
                GL.GenerateMipmap(texture.MipMapTarget);

            GL.BindTexture(texture.Target, 0);
        }
    }
}
