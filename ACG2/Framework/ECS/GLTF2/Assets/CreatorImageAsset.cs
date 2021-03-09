using Framework.Assets.Textures;
using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Mathematics;
using SharpGLTF.Schema2;
using OpenTK.Graphics.OpenGL;
using Framework.Assets.Materials;
using ImageMagick;

namespace Framework.ECS.GLTF2.Assets
{
    public static class CreatorImageAsset
    {
        public static ImageAsset Create(Image gltfImage)
        {
            var image = new ImageAsset(gltfImage.Name);
            image.SourceImage = new MagickImage(gltfImage.Content.Content.ToArray());
            image.Width = image.SourceImage.Width;
            image.Height = image.SourceImage.Height;
            image.Data = image.SourceImage.ToByteArray();

            if (image.SourceImage.ChannelCount == 4)
            {
                image.Format = PixelFormat.Rgba;
                image.InternalFormat = PixelInternalFormat.Rgba;
            }
            else
            {
                image.Format = PixelFormat.Rgb;
                image.InternalFormat = PixelInternalFormat.Rgba;
            }

            return image;
        }
    }
}
