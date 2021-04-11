using Framework.Assets.Materials;
using Framework.Assets.Shader;
using Framework.Assets.Textures;
using Framework.Assets.Verticies;
using Framework.ECS.Components.Transform;
using System.Collections.Generic;

namespace Framework.ECS.Components.Scene
{
    public struct RenderDataComponent
    {
        /// <summary>
        /// 
        /// </summary>
        public static RenderDataComponent Default => new RenderDataComponent()
        {
            Graph = new Dictionary<ShaderProgramAsset, Dictionary<MaterialAsset, Dictionary<TransformComponent, List<VertexPrimitiveAsset>>>>(),
            Shaders = new List<ShaderProgramAsset>(),
            Materials = new List<MaterialAsset>(),
            Primitves = new List<VertexPrimitiveAsset>(),
            Transforms = new List<TransformComponent>()
        };

        public Dictionary<ShaderProgramAsset,
                Dictionary<MaterialAsset,
                    Dictionary<TransformComponent,
                        List<VertexPrimitiveAsset>>>> Graph { get; set; }

        public List<MaterialAsset> Materials;
        public List<TextureBaseAsset> Textures;
        public List<ShaderProgramAsset> Shaders;
        public List<TransformComponent> Transforms;
        public List<VertexPrimitiveAsset> Primitves;
    }
}
