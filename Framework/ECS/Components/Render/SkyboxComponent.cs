using Framework.Assets.Materials;
using Framework.Assets.Shader;
using Framework.Assets.Verticies;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.ECS.Components.Render
{
    public class SkyboxComponent : IComponent
    {
        public ShaderProgramAsset Shader { get; set; }
        public MaterialAsset Material { get; set; }
        public MeshAsset Mesh { get; set; }
    }
}
