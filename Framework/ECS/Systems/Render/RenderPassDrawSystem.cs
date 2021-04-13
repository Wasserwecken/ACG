using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Materials;
using Framework.Assets.Shader;
using Framework.Assets.Shader.Block;
using Framework.Assets.Shader.Block.Data;
using Framework.Assets.Verticies;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Transform;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Linq;

namespace Framework.ECS.Systems.Render
{
    [With(typeof(RenderPassDataComponent))]
    public class RenderPassDrawSystem : AEntitySetSystem<bool>
    {
        private readonly Entity _worldComponents;
        private readonly ShaderBlockSingle<ShaderViewSpace> _viewSpaceBlock;
        private readonly ShaderBlockSingle<ShaderPrimitiveSpace> _primitiveSpaceBlock;

        /// <summary>
        /// 
        /// </summary>
        public RenderPassDrawSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;

            _viewSpaceBlock = new ShaderBlockSingle<ShaderViewSpace>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
            _primitiveSpaceBlock = new ShaderBlockSingle<ShaderPrimitiveSpace>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, in Entity entity)
        {
            var renderData = entity.Get<RenderPassDataComponent>();

            _viewSpaceBlock.Data = renderData.ViewSpace;
            _viewSpaceBlock.PushToGPU();


            GL.ClearColor(.2f, .2f, .2f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            foreach (var shaderRelation in renderData.Graph)
            {
                UseShader(shaderRelation.Key);

                foreach (var materialRelation in shaderRelation.Value)
                {
                    UseMaterial(materialRelation.Key);
                    SetUniforms(materialRelation.Key, shaderRelation.Key);

                    foreach (var transformRelation in materialRelation.Value)
                    {
                        _primitiveSpaceBlock.Data = CreatePrimitiveSpace(transformRelation.Key, renderData.WorldSpaceInverse, renderData.Projection);
                        _primitiveSpaceBlock.PushToGPU();

                        foreach (var primitive in transformRelation.Value)
                            Draw(primitive);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void UseShader(ShaderProgramAsset shader)
        {
            GL.UseProgram(shader.Handle);
            foreach (var block in ShaderBlockRegister.Blocks)
                if (shader.IdentifierToLayout.TryGetValue(block.Name, out var blockLayout))
                    GL.BindBufferBase(block.Target, blockLayout, block.Handle);
        }

        /// <summary>
        /// 
        /// </summary>
        private void UseMaterial(MaterialAsset material)
        {
            GL.ShadeModel(material.Model);
            GL.FrontFace(material.FaceDirection);

            if (material.IsDepthTesting)
            {
                GL.Enable(EnableCap.DepthTest);
                GL.DepthFunc(material.DepthTest);
            }
            else
                GL.Disable(EnableCap.DepthTest);

            if (material.IsCulling)
            {
                GL.Enable(EnableCap.CullFace);
                GL.CullFace(material.CullingMode);
            }
            else
                GL.Disable(EnableCap.CullFace);

            if (material.IsTransparent)
            {
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(material.SourceBlend, material.DestinationBlend);
            }
            else
                GL.Disable(EnableCap.Blend);
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetUniforms(MaterialAsset material, ShaderProgramAsset shader)
        {
            foreach (var uniform in material.UniformFloats)
                if (shader.IdentifierToLayout.TryGetValue(uniform.Key, out var layout))
                    GL.Uniform1(layout, uniform.Value);

            foreach (var uniform in material.UniformVecs)
                if (shader.IdentifierToLayout.TryGetValue(uniform.Key, out var layout))
                    GL.Uniform4(layout, uniform.Value);

            foreach (var uniform in material.UniformMats)
                if (shader.IdentifierToLayout.TryGetValue(uniform.Key, out var layout))
                {
                    var foo = uniform.Value;
                    GL.UniformMatrix4(layout, false, ref foo);
                }

            foreach (var uniform in material.UniformTextures)
                if (shader.IdentifierToLayout.TryGetValue(uniform.Key, out var layout))
                {
                    GL.Uniform1(layout, layout);
                    GL.ActiveTexture(TextureUnit.Texture0 + layout);
                    GL.BindTexture(uniform.Value.Target, uniform.Value.Handle);
                }

            foreach (var uniformTexture in shader.Uniforms.Where
                (f => f.Type == ActiveUniformType.Sampler2D && !material.UniformTextures.ContainsKey(f.Name)))
            {
                GL.Uniform1(uniformTexture.Layout, uniformTexture.Layout);
                GL.ActiveTexture(TextureUnit.Texture0 + uniformTexture.Layout);

                if (uniformTexture.Name.ToLower().Contains("normal"))
                    GL.BindTexture(Defaults.Texture.Normal.Target, Defaults.Texture.Normal.Handle);
                else
                    GL.BindTexture(Defaults.Texture.White.Target, Defaults.Texture.White.Handle);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private ShaderPrimitiveSpace CreatePrimitiveSpace(TransformComponent primitiveTransform, Matrix4 worldSpaceInverse, Matrix4 projection)
        {
            return new ShaderPrimitiveSpace
            {
                LocalToWorld = primitiveTransform.WorldSpace,
                LocalToView = primitiveTransform.WorldSpace * worldSpaceInverse,
                LocalToProjection = primitiveTransform.WorldSpace * worldSpaceInverse * projection,

                LocalToWorldRotation = primitiveTransform.WorldSpace.ClearScale(),
                LocalToViewRotation = (primitiveTransform.WorldSpace * worldSpaceInverse).ClearScale().ClearTranslation(),
                LocalToProjectionRotation = (primitiveTransform.WorldSpace * worldSpaceInverse).ClearScale().ClearTranslation() * projection,
            };
        }

        /// <summary>
        /// 
        /// </summary>
        private void Draw(VertexPrimitiveAsset primitive)
        {
            GL.BindVertexArray(primitive.Handle);
            GL.PolygonMode(MaterialFace.FrontAndBack, primitive.Mode);

            if (primitive.IndicieBuffer != null)
                GL.DrawElements(primitive.Type, primitive.IndicieBuffer.Indicies.Length, DrawElementsType.UnsignedInt, 0);
            else
                GL.DrawArrays(primitive.Type, 0, primitive.ArrayBuffer.ElementCount);
        }
    }
}
