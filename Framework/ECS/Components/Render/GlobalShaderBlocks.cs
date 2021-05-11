using Framework.Assets.Shader.Block;
using Framework.Assets.Shader.Block.Data;
using System.Collections.Generic;

namespace Framework.ECS.Components.Render
{
    public struct GlobalShaderBlocksComponent
    {
        public ShaderBlockArray<ShaderDirectionalLight> DirectionalLights;
        public ShaderBlockArray<ShaderPointLight> PointLights;
        public ShaderBlockArray<ShaderSpotLight> SpotLights;
        public ShaderBlockSingle<ShaderTime> Time;
    }
}
