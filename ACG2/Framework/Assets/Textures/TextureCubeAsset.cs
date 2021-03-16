using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Assets.Textures
{
    public class TextureCubeAsset : TextureBaseAsset
    {
        public TextureWrapMode WrapModeR { get; set; }
        public override TextureTarget Target => TextureTarget.TextureCubeMap;
        public override GenerateMipmapTarget MipMapTarget => GenerateMipmapTarget.TextureCubeMap;
        public ImageAsset[] Images;

        /// <summary>
        /// 
        /// </summary>
        public TextureCubeAsset(string name) : base(name)
        {
            WrapModeR = TextureWrapMode.Repeat;
            Images = new ImageAsset[6];
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void GLTexImage()
        {
            for(int i = 0; i < 6; i++)
                GL.TexImage2D(
                    TextureTarget.TextureCubeMapPositiveX + i,
                    0,
                    Images[i].InternalFormat,
                    Images[i].Width,
                    Images[i].Height,
                    0,
                    Images[i].Format,
                    Images[i].PixelType,
                    Images[i].Data
                );
            
            GL.TexParameter(Target, TextureParameterName.TextureWrapR, (int)WrapModeR);
        }
    }
}
