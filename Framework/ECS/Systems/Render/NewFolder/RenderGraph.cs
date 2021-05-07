using Framework.Assets.Materials;
using Framework.Assets.Shader;
using Framework.Assets.Shader.Block;
using Framework.Assets.Shader.Block.Data;
using Framework.Assets.Verticies;
using Framework.ECS.Components.Transform;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.ECS.Systems.Render
{
    public class RenderGraph : Dictionary<ShaderProgramAsset, Dictionary<MaterialAsset, Dictionary<TransformComponent, List<VertexPrimitiveAsset>>>>
    {
        /// <summary>
        /// 
        /// </summary>
        public void Draw(ShaderViewSpace viewSpace)
        {
            ShaderBlockSingle<ShaderViewSpace>.Instance.Data = viewSpace;
            ShaderBlockSingle<ShaderViewSpace>.Instance.PushToGPU();

            foreach (var shaderRelation in this)
            {
                Renderer.UseShader(shaderRelation.Key);
                foreach (var materialRelation in shaderRelation.Value)
                {
                    Renderer.UseMaterial(materialRelation.Key, shaderRelation.Key);
                    foreach (var transformRelation in materialRelation.Value)
                    {
                        ShaderBlockSingle<ShaderPrimitiveSpace>.Instance.Data = Renderer.CreatePrimitiveSpace(transformRelation.Key, viewSpace);
                        ShaderBlockSingle<ShaderPrimitiveSpace>.Instance.PushToGPU();

                        foreach (var primitive in transformRelation.Value)
                            Renderer.Draw(primitive);
                    }
                }
            }
        }
    }
}
