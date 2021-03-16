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

namespace Framework.ECS.Systems.Render
{
    public class RenderSystem : ISystem
    {
        private readonly ShaderBlockSingle<ShaderRenderSpace> _spaceBlock;
        
        /// <summary>
        /// 
        /// </summary>
        public RenderSystem()
        {
            _spaceBlock = new ShaderBlockSingle<ShaderRenderSpace>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Run(IEnumerable<Entity> entities, IEnumerable<IComponent> sceneComponents)
        {
            var renderDataComponent = sceneComponents.First(f => f is RenderDataComponent) as RenderDataComponent;
            var aspectRatioComponent = sceneComponents.First(f => f is AspectRatioComponent) as AspectRatioComponent;
            var cameras = entities.Where(f => f.HasAnyComponents(typeof(PerspectiveCameraComponent)));

            foreach(var cameraEntity in cameras)
            {
                var cameraData = cameraEntity.GetComponent<PerspectiveCameraComponent>();
                var cameraTransform = cameraEntity.GetComponent<TransformComponent>();
                SetPerspectiveData(cameraData, cameraTransform, aspectRatioComponent);

                foreach(var shaderRelation in renderDataComponent.Graph)
                {
                    UseShader(shaderRelation.Key);

                    foreach(var materialRelation in shaderRelation.Value)
                    {
                        UseMaterial(materialRelation.Key);
                        SetUniforms(materialRelation.Key, shaderRelation.Key);

                        foreach (var transformRelation in materialRelation.Value)
                        {
                            SetPrimitiveData(transformRelation.Key, cameraTransform);
                            _spaceBlock.PushToGPU();

                            foreach (var primitive in transformRelation.Value)
                                Draw(primitive);
                        }
                    }
                }

                if (sceneComponents.Has<SkyboxComponent>(out var skyboxComponent))
                {
                    UseShader(skyboxComponent.Shader);
                    UseMaterial(skyboxComponent.Material);
                    SetUniforms(skyboxComponent.Material, skyboxComponent.Shader);

                    foreach (var primitive in skyboxComponent.Mesh.Primitives)
                        Draw(primitive);
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

            if (material. IsDepthTesting)
                GL.Enable(EnableCap.DepthTest);

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
                    GL.BindTexture(Default.Texture.Normal.Target, Default.Texture.Normal.Handle);
                else
                    GL.BindTexture(Default.Texture.White.Target, Default.Texture.White.Handle);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetPerspectiveData(PerspectiveCameraComponent camera, TransformComponent transform, AspectRatioComponent aspect)
        {
            var cameraProjectionSpace = Matrix4.CreatePerspectiveFieldOfView(
               MathHelper.DegreesToRadians(camera.FieldOfView),
               aspect.Ratio,
               camera.NearClipping,
               camera.FarClipping
           );

            _spaceBlock.Data.WorldToView = transform.WorldSpaceInverse;
            _spaceBlock.Data.ProjectionSpace = cameraProjectionSpace;
            _spaceBlock.Data.ViewPosition = new Vector4(transform.Position, 1);
            _spaceBlock.Data.ViewPosition = new Vector4(transform.Forward, 0);

            GL.ClearColor(camera.ClearColor.X, camera.ClearColor.Y, camera.ClearColor.Z, camera.ClearColor.W);
            GL.Clear(camera.ClearMask);
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetPrimitiveData(TransformComponent primitiveTransform, TransformComponent cameraTransform)
        {
            _spaceBlock.Data.LocalToView = primitiveTransform.WorldSpace * cameraTransform.WorldSpaceInverse;
            _spaceBlock.Data.LocalToViewRotation = _spaceBlock.Data.LocalToView.ClearScale().ClearRotation();

            _spaceBlock.Data.LocalToProjection = primitiveTransform.WorldSpace * cameraTransform.WorldSpaceInverse;
            _spaceBlock.Data.LocalToProjection *= _spaceBlock.Data.ProjectionSpace;
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
