using Framework.Assets.Textures;
using OpenTK.Graphics.OpenGL;

namespace Framework.Assets.Framebuffer
{
    public class FrameBufferTextureAsset : TextureBaseAsset
    {
        public override TextureTarget Target => TextureTarget.Texture2D;
        public override GenerateMipmapTarget MipMapTarget => GenerateMipmapTarget.Texture2D;

        /// <summary>
        /// 
        /// </summary>
        public FrameBufferTextureAsset(string name) : base(name)
        {
            MinFilter = TextureMinFilter.Nearest;
            MagFilter = TextureMagFilter.Nearest;
        }
    }
}