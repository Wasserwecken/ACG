using ACG.Framework.Assets;
using ImageMagick;
using OpenTK.Graphics.OpenGL;

namespace Framework.Assets.Textures
{
    public abstract class TextureBaseAsset
    {
        public int Handle;
        public string Name;
        public bool GenerateMipMaps;
        
        public abstract TextureTarget Target { get; }
        public abstract GenerateMipmapTarget MipMapTarget { get; }

        public PixelType PixelType;
        public PixelFormat Format;
        public PixelInternalFormat InternalFormat;

        public TextureWrapMode WrapModeS;
        public TextureWrapMode WrapModeT;
        public TextureMinFilter MinFilter;
        public TextureMagFilter MagFilter;
        

        /// <summary>
        /// 
        /// </summary>
        public TextureBaseAsset(string name)
        {
            AssetRegister.Textures.Add(this);

            Name = name;
            GenerateMipMaps = true;

            PixelType = PixelType.UnsignedShort;
            Format = PixelFormat.Rgba;
            InternalFormat = (PixelInternalFormat)Format;

            WrapModeS = TextureWrapMode.Repeat;
            WrapModeT = TextureWrapMode.Repeat;
            MinFilter = TextureMinFilter.Linear;
            MagFilter = TextureMagFilter.Linear;
        }


        /// <summary>
        /// 
        /// </summary>
        protected void SetPixelInfos(MagickImage image)
        {
            if (image != null)
            {
                switch (image.ChannelCount)
                {
                    case 1:
                        Format = PixelFormat.Red;
                        break;
                    case 2:
                        Format = PixelFormat.Rg;
                        break;
                    case 3:
                        Format = PixelFormat.Rgb;
                        break;
                    case 4:
                        Format = PixelFormat.Rgba;
                        break;
                }

                PixelType = PixelType.UnsignedShort;
                InternalFormat = (PixelInternalFormat)Format;
            }
        }
    }
}
