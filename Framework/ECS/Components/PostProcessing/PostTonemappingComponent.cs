using Framework.Assets.Framebuffer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.ECS.Components.PostProcessing
{
    public struct PostTonemappingComponent
    {
        public FramebufferAsset Buffer;
        public float Exposure;
    }
}
