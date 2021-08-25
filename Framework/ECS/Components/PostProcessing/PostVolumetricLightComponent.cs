using Framework.Assets.Framebuffer;

namespace Framework.ECS.Components.PostProcessing
{
    public struct PostVolumetricLightComponent
    {
        public FramebufferAsset SamplingBuffer;
        public FramebufferAsset ResultBuffer;
        public float Strength;
    }
}
