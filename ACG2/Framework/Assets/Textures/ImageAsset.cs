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
        public MagickImage SourceImage;

        /// <summary>
        /// 
        /// </summary>
        public ImageAsset(string name)
        {
            Name = name;

            PixelType = PixelType.UnsignedShort;
            Format = PixelFormat.Rgba;
            InternalFormat = (PixelInternalFormat)Format;
        }
    }
}
