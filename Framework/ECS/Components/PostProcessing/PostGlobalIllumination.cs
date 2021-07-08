using Framework.Assets.Framebuffer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.ECS.Components.PostProcessing
{
    public struct PostGlobalIllumination
    {
        public int SampleBufferLength;
        public FramebufferAsset TracingBuffer;
        public FramebufferAsset SampleBuffer;
        public FramebufferAsset BlurBufferA;
        public FramebufferAsset BlurBufferB;
    }
}
