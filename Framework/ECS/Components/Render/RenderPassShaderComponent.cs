using Framework.Assets.Materials;
using Framework.Assets.Shader;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.ECS.Components.Render
{
    public struct RenderPassShaderComponent
    {
        public ShaderProgramAsset Shader;
        public MaterialAsset Material;
    }
}
