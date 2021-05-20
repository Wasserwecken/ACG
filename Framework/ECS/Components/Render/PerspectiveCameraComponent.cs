using System.Diagnostics;
using Framework.Assets.Shader.Block;
using Framework.Assets.Framebuffer;
using Framework.Assets.Textures;

namespace Framework.ECS.Components.Render
{
    [DebuggerDisplay("FieldOfView: {FieldOfView}, Clipping: {NearClipping} - {FarClipping}")]
    public struct PerspectiveCameraComponent
    {
        public TextureCubeAsset Skybox;
        public FramebufferAsset DeferredGBuffer;
        public FramebufferAsset DeferredLightBuffer;
        public ShaderViewSpaceBlock ShaderViewSpace;
        public ShaderDeferredViewBlock ShaderDeferredView;
        public float NearClipping;
        public float FarClipping;
        public float FieldOfView;
    }
}
