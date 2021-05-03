using DefaultEcs;
using Framework.Assets.Framebuffer;
using Framework.Assets.Materials;
using Framework.Assets.Shader;
using Framework.Assets.Shader.Block.Data;
using Framework.Assets.Verticies;
using Framework.ECS.Components.Transform;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.ECS.Components.Render
{
    public struct RenderPassDataComponent
    {
        public FramebufferAsset FrameBuffer;
    }
}
