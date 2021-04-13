using Framework.Assets.Materials;
using Framework.Assets.Shader;

namespace Framework.ECS.Components.Render
{
    public struct RenderPassShaderComponent
    {
        public ShaderProgramAsset Shader;
        public MaterialAsset Material;
    }
}
