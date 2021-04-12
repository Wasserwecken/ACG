using System.Collections.Generic;

namespace Framework.Assets.Framebuffer
{
    public class FramebufferAsset
    {
        public int Handle { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public List<FrameBufferTextureAsset> TextureTargets { get; set; }
        public List<FramebufferStorageAsset> StorageTargets { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public FramebufferAsset()
        {
            TextureTargets = new List<FrameBufferTextureAsset>();
            StorageTargets = new List<FramebufferStorageAsset>();
        }
    }
}
