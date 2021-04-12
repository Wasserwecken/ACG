using DefaultEcs;
using Framework.Assets.Materials;
using Framework.Assets.Shader;
using Framework.Assets.Verticies;
using Framework.ECS.Components.Transform;
using System.Collections.Generic;

namespace Framework.ECS.Components.Render
{
    public struct RenderPassGraphComponent
    {
        public List<Entity> Renderables;
        public Dictionary<ShaderProgramAsset,
                    Dictionary<MaterialAsset,
                        Dictionary<TransformComponent,
                            List<VertexPrimitiveAsset>>>> Graph;
    }
}
