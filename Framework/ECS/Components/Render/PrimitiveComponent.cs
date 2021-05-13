using Framework.Assets.Materials;
using Framework.Assets.Shader;
using Framework.Assets.Shader.Block;
using Framework.Assets.Verticies;

namespace Framework.ECS.Components.Render
{
    public struct PrimitiveComponent
    {
        public bool IsShadowCaster;
        public ShaderPrimitiveSpaceBlock ShaderSpaceBlock;
        public VertexPrimitiveAsset Verticies;
        public ShaderProgramAsset Shader;
        public MaterialAsset Material;
    }
}
