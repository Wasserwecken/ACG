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
        private static readonly ShaderBlockSingle<ShaderViewSpace> _viewSpaceBlock;
        private static readonly ShaderBlockSingle<ShaderPrimitiveSpace> _primitiveSpaceBlock;

        /// <summary>
        /// 
        /// </summary>
        static RenderGraph()
        {
            _viewSpaceBlock = new ShaderBlockSingle<ShaderViewSpace>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
            _primitiveSpaceBlock = new ShaderBlockSingle<ShaderPrimitiveSpace>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Draw(ShaderViewSpace viewSpace)
        {
            _viewSpaceBlock.Data = viewSpace;
            _viewSpaceBlock.PushToGPU();

            foreach (var shaderRelation in this)
            {
                Renderer.UseShader(shaderRelation.Key);
                foreach (var materialRelation in shaderRelation.Value)
                {
                    Renderer.UseMaterial(materialRelation.Key, shaderRelation.Key);
                    foreach (var transformRelation in materialRelation.Value)
                    {
                        _primitiveSpaceBlock.Data = Renderer.CreatePrimitiveSpace(transformRelation.Key, viewSpace);
                        _primitiveSpaceBlock.PushToGPU();

                        foreach (var primitive in transformRelation.Value)
                            Renderer.Draw(primitive);
                    }
                }
            }
        }
    }
}
