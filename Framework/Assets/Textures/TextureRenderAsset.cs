using Framework.Assets.Framebuffer;
using OpenTK.Graphics.OpenGL;

namespace Framework.Assets.Textures
{
    public class TextureRenderAsset : TextureBaseAsset
    {
        public override TextureTarget Target => TextureTarget.Texture2D;
        public override GenerateMipmapTarget MipMapTarget => GenerateMipmapTarget.Texture2D;

        public int Width { get; set; }
        public int Height { get; set; }
        public FramebufferAttachment Attachment { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public TextureRenderAsset(string name) : base(name)
        {
            AnisotropicFilter = 0f;
            GenerateMipMaps = false;
            MinFilter = TextureMinFilter.Nearest;
            MagFilter = TextureMagFilter.Nearest;
        }
    }
}