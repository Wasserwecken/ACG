using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public abstract class Texture
    {
        public int Handle { get; private set; }
        public abstract TextureTarget Target { get; }
        public abstract GenerateMipmapTarget MipMapTarget { get; }
        public TextureWrapMode WrapMode { get; protected set; }
        public TextureMinFilter MinFilter { get; protected set; }
        public TextureMagFilter MagFilter { get; protected set; }
        public PixelType PixelType { get; protected set; }
        public PixelFormat Format { get; protected set; }
        public PixelInternalFormat InternalFormat { get; protected set; }
        public bool GenerateMipMaps { get; protected set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public Texture()
        {
            GenerateMipMaps = true;
            
            Format = PixelFormat.Rgb;
            InternalFormat = PixelInternalFormat.Rgb;
            PixelType = PixelType.UnsignedShort;

            WrapMode = TextureWrapMode.Clamp;
            MinFilter = TextureMinFilter.Linear;
            MagFilter = TextureMagFilter.Linear;
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Create(ushort[] pixels)
        {
            Handle = GL.GenTexture();
            GL.BindTexture(Target, Handle);

            GL.TexParameter(Target, TextureParameterName.TextureWrapS, (int)WrapMode);
            GL.TexParameter(Target, TextureParameterName.TextureWrapT, (int)WrapMode);
            GL.TexParameter(Target, TextureParameterName.TextureMinFilter, (int)MinFilter);
            GL.TexParameter(Target, TextureParameterName.TextureMagFilter, (int)MagFilter);

            GLTexImage(pixels);

            if (GenerateMipMaps)
                GL.GenerateMipmap(MipMapTarget);

            GL.BindTexture(Target, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        protected abstract void GLTexImage(ushort[] pixels);
    }
}
