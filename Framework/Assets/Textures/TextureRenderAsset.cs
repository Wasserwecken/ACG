using Framework.Assets.Framebuffer;
using OpenTK.Graphics.OpenGL;

namespace Framework.Assets.Textures
{
    public class TextureRenderAsset : TextureBaseAsset
    {
        public override TextureTarget Target => TextureTarget.Texture2D;
        public override GenerateMipmapTarget MipMapTarget => GenerateMipmapTarget.Texture2D;

        public int Width { get; }
        public int Height { get; }
        public FramebufferAttachment Attachment { get; }
        
        /// <summary>
        /// 
        /// </summary>
        public TextureRenderAsset(string name, FramebufferAttachment attachment, int width, int height) : base(name)
        {
            Width = width;
            Height = height;

            Attachment = attachment;
            MinFilter = TextureMinFilter.Nearest;
            MagFilter = TextureMagFilter.Nearest;
        }
    }
}