using Framework.Assets.Materials;
using Framework.Assets.Shader;
using Framework.Assets.Verticies;
using System.Collections.Generic;
using System.Diagnostics;

namespace Framework.ECS.Components.Render
{
    [DebuggerDisplay("Mesh: {Mesh?.Name}, Materials: {Materials.Count}, Shaders: {Shaders.Count}")]
    public class MeshRendererComponent : IComponent
    {
        public MeshAsset Mesh { get; set; }
        public List<MaterialAsset> Materials { get; set; }
        public List<ShaderProgramAsset> Shaders { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MeshRendererComponent()
        {
            Materials = new List<MaterialAsset>();
            Shaders = new List<ShaderProgramAsset>();
        }
    }
}
