using System.Collections.Generic;

namespace Framework.Assets.Framebuffer
{
    public static class FramebufferRegister
    {
        public static List<FramebufferAsset> Buffers { get; }

        /// <summary>
        /// 
        /// </summary>
        static FramebufferRegister()
        {
            Buffers = new List<FramebufferAsset>();
        }
    }
}
