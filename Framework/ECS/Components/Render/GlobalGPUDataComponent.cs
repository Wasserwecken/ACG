using Framework.Assets.Materials;
using Framework.Assets.Shader;
using Framework.Assets.Textures;
using Framework.Assets.Verticies;
using Framework.ECS.Components.Transform;
using System.Collections.Generic;

namespace Framework.ECS.Components.Render
{
    public struct GlobalGPUDataComponent
    {
        public static GlobalGPUDataComponent Default => new GlobalGPUDataComponent()
        {
            Materials = new HashSet<MaterialAsset>(),
            Textures = new HashSet<TextureBaseAsset>(),
            Shaders = new HashSet<ShaderProgramAsset>(),
            Primitives = new HashSet<VertexPrimitiveAsset>()
        };

        public HashSet<MaterialAsset> Materials;
        public HashSet<TextureBaseAsset> Textures;
        public HashSet<ShaderProgramAsset> Shaders;
        public HashSet<VertexPrimitiveAsset> Primitives;
    }
}
