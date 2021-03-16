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
    }
}
