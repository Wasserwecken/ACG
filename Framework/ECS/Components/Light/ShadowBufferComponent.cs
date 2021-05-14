using Framework.Assets.Framebuffer;
using Framework.Assets.Shader.Block;
using Framework.Assets.Textures;
using System.Diagnostics;

namespace Framework.ECS.Components.Light
{
    [DebuggerDisplay("Lights: {Data.Length}, BufferTextures: {ShadowBuffer.Textures.Count}, BufferSize: {ShadowBuffer.Width}, {ShadowBuffer.Height}")]
    public struct ShadowBufferComponent
    {
        public FramebufferAsset FramebufferBuffer;
        public TextureSpace TextureAtlas;
        public ShaderDirectionalShadowBlock DirectionalBlock;
        public ShaderPointShadowBlock PointBlock;
        public ShaderSpotShadowBlock SpotBlock;
    }
}
