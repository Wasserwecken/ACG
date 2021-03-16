using Framework.Assets.Textures;
using SharpGLTF.Schema2;
using ImageMagick;

namespace Framework.ECS.GLTF2.Assets
{
    public static class CreatorImageAsset
    {
        public static ImageAsset Create(Image gltfImage)
        {
            if (gltfImage.Content.SourcePath != null)
                return Helper.LoadImage(gltfImage.Content.SourcePath);
            else
                return new ImageAsset(gltfImage.Name, new MagickImage(gltfImage.Content.Content.ToArray()));
        }
    }
}
