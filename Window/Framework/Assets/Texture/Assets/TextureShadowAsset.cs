using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public class TextureShadowAsset : TextureBase
    {
        public override TextureTarget Target => TextureTarget.Texture2D;
        public override GenerateMipmapTarget MipMapTarget => GenerateMipmapTarget.Texture2D;

        /// <summary>
        /// 
        /// </summary>
        public TextureShadowAsset() : base()
        {
            PixelType = PixelType.Float;
            Format = PixelFormat.DepthComponent;
            InternalFormat = PixelInternalFormat.DepthComponent;

            MinFilter = TextureMinFilter.Nearest;
            MagFilter = TextureMagFilter.Nearest;
        }
    }
}
