using Framework.Assets.Textures;
using ImageMagick;
using System.IO;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public static class Helper
    {
        /// <summary>
        /// 
        /// </summary>
        public static ImageAsset LoadImage(string path)
        {
            var name = Path.GetFileNameWithoutExtension(path);
            var source = new MagickImage(path);

            return new ImageAsset(name, source);
        }
    }
}
