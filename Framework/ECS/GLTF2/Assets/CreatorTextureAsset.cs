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
                WrapModeS = gltfTexture.Sampler != null ? (OpenTK.Graphics.OpenGL.TextureWrapMode)gltfTexture.Sampler.WrapS : OpenTK.Graphics.OpenGL.TextureWrapMode.Repeat,
                WrapModeT = gltfTexture.Sampler != null ? (OpenTK.Graphics.OpenGL.TextureWrapMode)gltfTexture.Sampler.WrapT : OpenTK.Graphics.OpenGL.TextureWrapMode.Repeat,
                MagFilter = gltfTexture.Sampler != null ? (TextureMagFilter)gltfTexture.Sampler.MagFilter : TextureMagFilter.Linear,
                MinFilter = gltfTexture.Sampler != null ? (TextureMinFilter)gltfTexture.Sampler.MinFilter : TextureMinFilter.Linear,
                Image = images[gltfTexture.PrimaryImage]
            };
        }
    }
}
