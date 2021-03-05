using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ImageMagick;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public static class ImageLoaderMagick
    {
        /// <summary>
        /// 
        /// </summary>
        public static ImageAsset Load(string filePath)
        {
            using var loadedImage = new MagickImage(filePath);
            loadedImage.Flip();

            return new ImageAsset
            {
                Name = Path.GetFileNameWithoutExtension(filePath),
                Width = loadedImage.Width,
                Height = loadedImage.Height,
                Channels = loadedImage.ChannelCount,
                Pixels = loadedImage.GetPixels().ToArray()
            };
        }
    }
}
