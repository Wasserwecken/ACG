using Framework.Assets.Materials;
using Framework.Assets.Shader;
using Framework.Assets.Verticies;
using System.Collections.Generic;
using System.Diagnostics;

namespace Framework.ECS.Components.Render
{
    [DebuggerDisplay("Mesh: {Mesh?.Name}, Materials: {Materials.Count}, Shaders: {Shaders.Count}")]
    public class MeshComponent : IComponent
    {
        public List<ShaderProgramAsset> Shaders { get; set; }
        public List<MaterialAsset> Materials { get; set; }
        public MeshAsset Mesh { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MeshComponent()
        {
            Materials = new List<MaterialAsset>();
            Shaders = new List<ShaderProgramAsset>();
        }
    }
}
