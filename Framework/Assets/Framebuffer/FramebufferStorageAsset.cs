using OpenTK.Graphics.OpenGL;

namespace Framework.Assets.Framebuffer
{
    public class FramebufferStorageAsset
    {
        public int Handle { get; set; }
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public FramebufferAttachment Attachment { get; set; }
        public RenderbufferTarget Target { get; set; }
        public RenderbufferStorage DataType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public FramebufferStorageAsset(string name)
        {
            Name = name;
            Target = RenderbufferTarget.Renderbuffer;
        }
    }
}
