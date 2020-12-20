using OpenTK.Graphics.OpenGL;
using ImageMagick;

namespace Framework
{
    public class Texture2D : Texture
    {
        public override TextureTarget Target => TextureTarget.Texture2D;
        public override GenerateMipmapTarget MipMapTarget => GenerateMipmapTarget.Texture2D;
        public ushort[] Pixels { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Texture2D(string path) : base()
        {
            using var image = new MagickImage(path);
            image.Flip();

            WrapMode = TextureWrapMode.Repeat;
            Width = image.Width;
            Height = image.Height;

            if (image.ChannelCount == 4)
            {
                Format = PixelFormat.Rgba;
                InternalFormat = PixelInternalFormat.Rgba;
            }

            Pixels = image.GetPixels().ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        public void PushToGPU()
        {
            PushToGPU(Pixels);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void GLTexImage(ushort[] pixels)
        {
            GL.TexImage2D(Target, 0, InternalFormat, Width, Height, 0, Format, PixelType, pixels);
        }
    }
}
