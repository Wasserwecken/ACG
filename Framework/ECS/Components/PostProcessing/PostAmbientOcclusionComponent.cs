using Framework.Assets.Framebuffer;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.ECS.Components.PostProcessing
{
    public struct PostAmbientOcclusionComponent
    {
        public FramebufferAsset BufferA;
        public FramebufferAsset BufferB;
        public float Radius;
        public float Bias;
        public float Strength;
        public Matrix4 Projection;
    }
}
