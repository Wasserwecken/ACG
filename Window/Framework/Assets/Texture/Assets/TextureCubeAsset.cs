using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public class TextureCubeAsset : TextureBase
    {
        public override TextureTarget Target => TextureTarget.TextureCubeMap;
        public override GenerateMipmapTarget MipMapTarget => GenerateMipmapTarget.TextureCubeMap;
        public ImageAsset[] Images;

        /// <summary>
        /// 
        /// </summary>
        public TextureCubeAsset() : base()
        {
            WrapMode = TextureWrapMode.ClampToEdge;
            MagFilter = TextureMagFilter.Linear;
            MinFilter = TextureMinFilter.Linear;

            Images = new ImageAsset[6];
        }
    }
}
