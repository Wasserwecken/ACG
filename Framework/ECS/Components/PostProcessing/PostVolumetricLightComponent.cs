using Framework.Assets.Framebuffer;
using OpenTK.Mathematics;

namespace Framework.ECS.Components.PostProcessing
{
    public struct PostVolumetricLightComponent
    {
        public FramebufferAsset SamplingBuffer;
        public FramebufferAsset BlurBuffer;
        public FramebufferAsset AddBuffer;
        public int SamplingClusterSize;
        public float SamplingMarchStepMaxSize;
        public float SamplingMarchStepMaxCount;
        public float SamplingBufferScale;
        public Vector4 VolumeColor;
        public float VolumeDensity;
        public float VolumeScattering;
        public float Strength;
    }
}
