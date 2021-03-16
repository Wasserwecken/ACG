using Framework.Assets.Materials;
using Framework.Assets.Shader;
using Framework.Assets.Textures;
using Framework.Assets.Verticies;
using Framework.ECS.Components.Transform;
using System.Collections.Generic;

namespace Framework.ECS.Components.Scene
{
    public class RenderDataComponent : IComponent
    {
        public Dictionary<ShaderProgramAsset,
                Dictionary<MaterialAsset,
                    Dictionary<TransformComponent,
                        List<VertexPrimitiveAsset>>>> Graph { get; set; }

        public List<Entity> Entities;
        public List<TextureBaseAsset> Textures;

        public List<ShaderProgramAsset> Shaders;
        public List<MaterialAsset> Materials;
        public List<TransformComponent> Transforms;
        public List<VertexPrimitiveAsset> Primitves;

        /// <summary>
        /// 
        /// </summary>
        public RenderDataComponent()
        {
            Entities = new List<Entity>();
            Shaders = new List<ShaderProgramAsset>();
            Materials = new List<MaterialAsset>();
            Transforms = new List<TransformComponent>();
            Primitves = new List<VertexPrimitiveAsset>();
            Graph = new Dictionary<ShaderProgramAsset, Dictionary<MaterialAsset, Dictionary<TransformComponent, List<VertexPrimitiveAsset>>>>();
        }
    }
}
