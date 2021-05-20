using Framework.Assets.Framebuffer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.ECS.Components.PostProcessing
{
    public struct PostBloomComponent
    {
        public FramebufferAsset BufferB;
        public FramebufferAsset BufferA;
        public float Threshold;
        public float Size;
        public int Samples;
    }
}
