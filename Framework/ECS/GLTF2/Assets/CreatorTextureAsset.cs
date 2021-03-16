using Framework.Assets.Textures;
using SharpGLTF.Schema2;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace Framework.ECS.GLTF2.Assets
{
    public static class CreatorTextureAsset
    {
        public static TextureBaseAsset Create(Texture gltfTexture, Dictionary<Image, ImageAsset> images)
        {
            return new Texture2DAsset(gltfTexture.Name)
            {
                WrapModeS = (OpenTK.Graphics.OpenGL.TextureWrapMode)gltfTexture.Sampler.WrapS,
                WrapModeT = (OpenTK.Graphics.OpenGL.TextureWrapMode)gltfTexture.Sampler.WrapT,
                MagFilter = (TextureMagFilter)gltfTexture.Sampler.MagFilter,
                MinFilter = (TextureMinFilter)gltfTexture.Sampler.MinFilter,
                Image = images[gltfTexture.PrimaryImage]
            };
        }
    }
}
