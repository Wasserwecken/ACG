﻿using Framework.Assets.Framebuffer;
using Framework.Assets.Shader.Block;
using Framework.Assets.Textures;
using System.Diagnostics;

namespace Framework.ECS.Components.Scene
{
    [DebuggerDisplay("Lights: {Data.Length}, BufferTextures: {ShadowBuffer.Textures.Count}, BufferSize: {ShadowBuffer.Width}, {ShadowBuffer.Height}")]
    public struct ReflectionBufferComponent
    {
        public int Size;
        public FramebufferAsset DeferredGBuffer;
        public FramebufferAsset DeferredLightBuffer;
        public TextureSpace TextureAtlas;
        public ShaderReflectionBlock ReflectionBlock;
    }
}
