using System;
using System.Linq;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Scene;
using Framework.ECS.Components.Transform;
using Framework.Assets.Shader.Block;

namespace Framework.ECS.Systems
{
    public class RenderSystem : ISystem
    {
        private ShaderBlockSingle<ShaderTime> _timeUniformBlock;
        private ShaderBlockSingle<ShaderRenderSpace> _renderSpaceUniformBlock;
        
        /// <summary>
        /// 
        /// </summary>
        public RenderSystem()
        {
            _timeUniformBlock = new ShaderBlockSingle<ShaderTime>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
            _renderSpaceUniformBlock = new ShaderBlockSingle<ShaderRenderSpace>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Update(IEnumerable<Entity> entities, IEnumerable<IComponent> sceneComponents)
        {
            var renderGraphComponent = sceneComponents.First(f => f is RenderGraphComponent) as RenderGraphComponent;
            var aspectRatioComponent = sceneComponents.First(f => f is AspectRatioComponent) as AspectRatioComponent;

            var time = sceneComponents.First(f => f is TimeComponent) as TimeComponent;
            _timeUniformBlock.Data.Total = time.Total;
            _timeUniformBlock.Data.TotalSin = time.TotalSin;
            _timeUniformBlock.Data.Frame = time.DeltaFrame;
            _timeUniformBlock.Data.Fixed = time.DeltaFixed;
            _timeUniformBlock.PushToGPU();

            var cameras = entities.Where(f => f.HasAnyComponents(typeof(PerspectiveCameraComponent)));
            foreach(var cameraEntity in cameras)
            {
                var cameraData = cameraEntity.GetComponent<PerspectiveCameraComponent>();
                var cameraTransform = cameraEntity.GetComponent<TransformComponent>();
                var cameraProjectionSpace = Matrix4.CreatePerspectiveFieldOfView(
                    MathHelper.DegreesToRadians(cameraData.FieldOfView),
                    aspectRatioComponent.Ratio,
                    cameraData.NearClipping,
                    cameraData.FarClipping
                );

                GL.ClearColor(cameraData.ClearColor.X, cameraData.ClearColor.Y, cameraData.ClearColor.Z, cameraData.ClearColor.W);
                GL.Clear(cameraData.ClearMask);

                foreach(var shaderRelation in renderGraphComponent.Graph)
                {
                    var shader = shaderRelation.Key;
                    shader.Use();

                    foreach(var materialRelation in shaderRelation.Value)
                    {
                        var material = materialRelation.Key;
                        material.Use();

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
                                if (uniform.Value.Handle <= 0) uniform.Value.PushToGPU();

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
                                GL.BindTexture(Default.Texture.DefaultNormal.Target, Default.Texture.DefaultNormal.Handle);
                            else
                                GL.BindTexture(Default.Texture.DefaultWhite.Target, Default.Texture.DefaultWhite.Handle);
                        }


                        foreach (var transformRelation in materialRelation.Value)
                        {
                            var primitiveTransform = transformRelation.Key;

                            _renderSpaceUniformBlock.Data.WorldToView = cameraTransform.WorldSpaceInverse;
                            _renderSpaceUniformBlock.Data.ViewPosition = new Vector4(cameraTransform.Position, 1);
                            _renderSpaceUniformBlock.Data.ViewPosition = new Vector4(cameraTransform.Forward, 0);

                            _renderSpaceUniformBlock.Data.LocalToView = primitiveTransform.WorldSpace * cameraTransform.WorldSpaceInverse;
                            _renderSpaceUniformBlock.Data.LocalToProjection = primitiveTransform.WorldSpace * cameraTransform.WorldSpaceInverse * cameraProjectionSpace;
                            _renderSpaceUniformBlock.Data.LocalToViewRotation = primitiveTransform.WorldSpace.ClearScale().ClearRotation() * cameraTransform.WorldSpaceInverse.ClearScale().ClearTranslation();
                            
                            _renderSpaceUniformBlock.PushToGPU();

                            foreach (var primitive in transformRelation.Value)
                                primitive.Draw();
                        }
                    }
                }
            }
        }
    }
}
