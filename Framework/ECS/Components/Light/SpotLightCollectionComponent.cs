﻿using Framework.Assets.Shader.Block.Data;
using Framework.Assets.Textures;
using System.Diagnostics;

namespace Framework.ECS.Components.Light
{
    [DebuggerDisplay("Lights: {Data.Length}, BufferTextures: {ShadowBuffer.Textures.Count}, BufferSize: {ShadowBuffer.Width}, {ShadowBuffer.Height}")]
    public struct SpotLightCollectionComponent
    {
        public ShaderSpotLight[] Data;
        public TextureSpace ShadowSpacer;
    }
}