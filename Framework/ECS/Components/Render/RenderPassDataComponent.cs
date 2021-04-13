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
        public Matrix4 Projection;
        public Matrix4 WorldSpaceInverse;
        public ShaderViewSpace ViewSpace;
        public EntitySet RenderableCandidates;

        public FramebufferAsset FrameBuffer;

        public List<Entity> Renderables;
        public Dictionary<ShaderProgramAsset,
                    Dictionary<MaterialAsset,
                        Dictionary<TransformComponent,
                            List<VertexPrimitiveAsset>>>> Graph;

    }
}
