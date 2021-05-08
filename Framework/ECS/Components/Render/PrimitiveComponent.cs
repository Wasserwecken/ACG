using Framework.Assets.Materials;
using Framework.Assets.Shader;
using Framework.Assets.Verticies;

namespace Framework.ECS.Components.Render
{
    public struct PrimitiveComponent
    {
        public bool IsShadowCaster;
        public VertexPrimitiveAsset Primitive;
        public ShaderProgramAsset Shader;
        public MaterialAsset Material;
    }
}
