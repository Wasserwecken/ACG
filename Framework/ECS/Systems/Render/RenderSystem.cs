using System.Linq;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Scene;
using Framework.ECS.Components.Transform;
using Framework.Assets.Shader.Block;
using Framework.Assets.Shader;
using Framework.Assets.Materials;
using Framework.Assets.Verticies;
using Framework.Assets.Shader.Block.Data;
using Framework.Assets.Textures;
using DefaultEcs.System;
using DefaultEcs;

namespace Framework.ECS.Systems.Render
{
    [With(typeof(PerspectiveCameraComponent))]
    public class RenderSystem : AEntitySetSystem<bool>
    {
        private readonly RenderDataComponent _renderData;
        private readonly AspectRatioComponent _aspectRatio;
        private readonly ShaderBlockSingle<ShaderViewSpace> _viewSpaceBlock;
        private readonly ShaderBlockSingle<ShaderPrimitiveSpace> _primitiveSpaceBlock;

        /// <summary>
        /// 
        /// </summary>
        public RenderSystem(World world, Entity worldComponents) : base(world)
        {
            _renderData = worldComponents.Get<RenderDataComponent>();
            _aspectRatio = worldComponents.Get<AspectRatioComponent>();

            _viewSpaceBlock = new ShaderBlockSingle<ShaderViewSpace>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
            _primitiveSpaceBlock = new ShaderBlockSingle<ShaderPrimitiveSpace>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, in Entity entity)
        {
            var cameraData = entity.Get<PerspectiveCameraComponent>();
            var cameraTransform = entity.Get<TransformComponent>();
            var projectionSpace = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(cameraData.FieldOfView), _aspectRatio.Ratio, cameraData.NearClipping, cameraData.FarClipping);

            TextureBaseAsset skyboxTexture = Defaults.Texture.SkyboxCoast;
            if (entity.Has<MeshComponent>())
                skyboxTexture = entity.Get<MeshComponent>().Materials[0].UniformTextures[Definitions.Shader.Uniform.ReflectionMap];

            UseCamera(cameraData);
            _viewSpaceBlock.Data = CreateViewSpace(cameraTransform, projectionSpace);
            _viewSpaceBlock.PushToGPU();

            foreach (var shaderRelation in _renderData.Graph)
            {
                UseShader(shaderRelation.Key);

                foreach (var materialRelation in shaderRelation.Value)
                {
                    materialRelation.Key.SetUniform(Definitions.Shader.Uniform.ReflectionMap, skyboxTexture);
                    UseMaterial(materialRelation.Key);
                    SetUniforms(materialRelation.Key, shaderRelation.Key);

                    foreach (var transformRelation in materialRelation.Value)
                    {
                        _primitiveSpaceBlock.Data = CreatePrimitiveSpace(transformRelation.Key, cameraTransform, projectionSpace);
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
        private void UseCamera(PerspectiveCameraComponent camera)
        {
            GL.ClearColor(camera.ClearColor.X, camera.ClearColor.Y, camera.ClearColor.Z, camera.ClearColor.W);
            GL.Clear(camera.ClearMask);
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
        private ShaderViewSpace CreateViewSpace(TransformComponent transform, Matrix4 projection)
        {
            return new ShaderViewSpace
            {
                WorldToView = transform.WorldSpaceInverse,
                WorldToProjection = transform.WorldSpaceInverse * projection,

                WorldToViewRotation = transform.WorldSpaceInverse.ClearScale().ClearTranslation(),
                WorldToProjectionRotation = transform.WorldSpaceInverse.ClearScale().ClearTranslation() * projection,

                ViewPosition = new Vector4(transform.Position, 1),
                ViewDirection = new Vector4(-transform.Forward, 0)
            };
        }

        /// <summary>
        /// 
        /// </summary>
        private ShaderPrimitiveSpace CreatePrimitiveSpace(TransformComponent primitiveTransform, TransformComponent viewTransform, Matrix4 projection)
        {
            return new ShaderPrimitiveSpace
            {
                LocalToWorld = primitiveTransform.WorldSpace,
                LocalToView = primitiveTransform.WorldSpace * viewTransform.WorldSpaceInverse,
                LocalToProjection = primitiveTransform.WorldSpace * viewTransform.WorldSpaceInverse * projection,

                LocalToWorldRotation = primitiveTransform.WorldSpace.ClearScale(),
                LocalToViewRotation = (primitiveTransform.WorldSpace * viewTransform.WorldSpaceInverse).ClearScale().ClearTranslation(),
                LocalToProjectionRotation = (primitiveTransform.WorldSpace * viewTransform.WorldSpaceInverse).ClearScale().ClearTranslation() * projection,
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
