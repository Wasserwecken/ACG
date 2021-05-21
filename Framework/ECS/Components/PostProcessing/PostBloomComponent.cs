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
        public int Samples;
        public float ThresholdStart;
        public float ThresholdEnd;
        public float Intensity;
    }
}
