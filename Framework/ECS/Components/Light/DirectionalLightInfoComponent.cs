using Framework.Assets.Framebuffer;
using Framework.Assets.Shader.Block.Data;
using Framework.Assets.Textures;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.ECS.Components.Light
{
    public struct DirectionalLightInfoComponent
    {
        public ShaderDirectionalLight[] Data;
        public FramebufferAsset ShadowBuffer;
        public TextureSpace ShadowSpacer;
    }
}
