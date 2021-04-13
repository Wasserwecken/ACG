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
        public MagickImage Image { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ImageAsset(string name)
        {
            Name = name;

            Width = 1;
            Height = 1;
        }

        /// <summary>
        /// 
        /// </summary>
        public ImageAsset(string name, MagickImage image)
        {
            Name = name;
            Image = image;

            Width = image.Width;
            Height = image.Height;
            Data = image.GetPixels().ToArray();
        }
    }
}
