using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Assets.Textures
{
    public class Texture2DAsset : TextureBaseAsset
    {
        public override TextureTarget Target => TextureTarget.Texture2D;
        public override GenerateMipmapTarget MipMapTarget => GenerateMipmapTarget.Texture2D;
        public ImageAsset Image { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Texture2DAsset(string name) : base(name) { }

        /// <summary>
        /// 
        /// </summary>
        protected override void GLTexImage()
        {
            GL.TexImage2D(Target, 0, Image.InternalFormat, Image.Width, Image.Height, 0, Image.Format, Image.PixelType, Image.Data);
        }
    }
}
