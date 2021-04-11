using Framework.Assets.Materials;
using Framework.Assets.Shader;
using Framework.Assets.Verticies;
using System.Collections.Generic;
using System.Diagnostics;

namespace Framework.ECS.Components.Render
{
    [DebuggerDisplay("Mesh: {Mesh?.Name}, Materials: {Materials.Count}, Shaders: {Shaders.Count}")]
    public struct MeshComponent
    {
        /// <summary>
        /// 
        /// </summary>
        public static MeshComponent Default => new MeshComponent()
        {
            Materials = new List<MaterialAsset>(),
            Shaders = new List<ShaderProgramAsset>()
        };

        public List<ShaderProgramAsset> Shaders { get; set; }
        public List<MaterialAsset> Materials { get; set; }
        public MeshAsset Mesh { get; set; }
    }
}
