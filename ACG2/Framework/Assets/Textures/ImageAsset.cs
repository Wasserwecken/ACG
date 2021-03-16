using ImageMagick;
using OpenTK.Graphics.OpenGL;

namespace Framework.Assets.Textures
{
    public class ImageAsset
    {
        public string Name;
        public int Width;
        public int Height;
        public ushort[] Data;
        public PixelType PixelType { get; set; }
        public PixelFormat Format { get; set; }
        public PixelInternalFormat InternalFormat { get; set; }
        public MagickImage SourceImage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ImageAsset(string name)
        {
            Name = name;

            Width = 1;
            Height = 1;

            PixelType = PixelType.UnsignedShort;
            Format = PixelFormat.Rgba;
            InternalFormat = (PixelInternalFormat)Format;
        }

        /// <summary>
        /// 
        /// </summary>
        public ImageAsset(string name, MagickImage sourceImage)
        {
            Name = name;
            SourceImage = sourceImage;

            UpdateImageData();
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateImageData()
        {
            if (SourceImage != null)
            {
                PixelType = PixelType.UnsignedShort;

                Width = SourceImage.Width;
                Height = SourceImage.Height;
                Data = SourceImage.GetPixels().ToArray();

                switch (SourceImage.ChannelCount)
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
                InternalFormat = (PixelInternalFormat)Format;
            }
        }
    }
}
