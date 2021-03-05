using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public abstract class TextureBase
    {
        public int Handle { get; set; }
        public string Name { get; set; }
        public abstract TextureTarget Target { get; }
        public abstract GenerateMipmapTarget MipMapTarget { get; }
        public TextureWrapMode WrapMode { get; set; }
        public TextureMinFilter MinFilter { get; set; }
        public TextureMagFilter MagFilter { get; set; }
        public PixelType PixelType { get; set; }
        public PixelFormat Format { get; set; }
        public PixelInternalFormat InternalFormat { get; set; }
        public bool GenerateMipMaps { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TextureBase()
        {
            GenerateMipMaps = true;

            Format = PixelFormat.Rgba;
            PixelType = PixelType.UnsignedShort;
            InternalFormat = (PixelInternalFormat)Format;

            WrapMode = TextureWrapMode.Repeat;
            MinFilter = TextureMinFilter.Linear;
            MagFilter = TextureMagFilter.Linear;
        }
    }
}
