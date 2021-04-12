using DefaultEcs;
using Framework.Assets.Shader;
using Framework.Assets.Shader.Block.Data;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.ECS.Components.Render
{
    public struct RenderPassViewComponent
    {
        public Matrix4 Projection;
        public Matrix4 WorldSpaceInverse;
        public ShaderViewSpace ViewSpace;
        public EntitySet RenderableCandidates;
        public ShaderProgramAsset UniformShader;
    }
}
