using System;
using System.Linq;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Scene;
using Framework.ECS.Components.Transform;
using Framework.Assets.Shader;
using Framework.Assets.Materials;
using Framework.Assets.Verticies;
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
            var aspectRatioComponent = sceneComponents.First(f => f is AspectRatioComponent) as AspectRatioComponent;

            var time = sceneComponents.First(f => f is TimeComponent) as TimeComponent;
            _timeUniformBlock.Data.Total = time.Total;
            _timeUniformBlock.Data.TotalSin = time.TotalSin;
            _timeUniformBlock.Data.Frame = time.DeltaFrame;
            _timeUniformBlock.Data.Fixed = time.DeltaFixed;
            _timeUniformBlock.PushToGPU();


            var meshes = entities.Where(f => f.HasAnyComponents(typeof(MeshRendererComponent)));
            var renderGraph = new Dictionary<ShaderProgramAsset, Dictionary<MaterialAsset, List<VertexPrimitiveAsset>>>();
            foreach(var mesh in meshes)
            {
                var meshComponent = mesh.GetComponent<MeshRendererComponent>();
                for (int i = 0; i < meshComponent.Mesh.Primitives.Count; i++)
                {
                    var shader = meshComponent.Shaders[meshComponent.Shaders.Count < i ? i : 0];
                    var material = meshComponent.Materials[meshComponent.Materials.Count < i ? i : 0];
                    var primitive = meshComponent.Mesh.Primitives[i];

                    if (renderGraph.TryGetValue(shader, out var materialRelation))
                    {
                        if (materialRelation.TryGetValue(material, out var primitives))
                            primitives.Add(primitive);
                        else
                            materialRelation.Add(material, new List<VertexPrimitiveAsset>() { primitive });
                    }
                    else
                    {
                        renderGraph.Add(shader, new Dictionary<MaterialAsset, List<VertexPrimitiveAsset>>()
                        {
                            { material, new List<VertexPrimitiveAsset>() { primitive } }
                        });
                    }

                    if (primitive.Handle <= 0) primitive.PushToGPU();
                }
            }


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

                _renderSpaceUniformBlock.Data.WorldToView = cameraTransform.WorldSpaceInverse;
                _renderSpaceUniformBlock.Data.ViewPosition = new Vector4(cameraTransform.Position, 1);
                _renderSpaceUniformBlock.Data.ViewPosition = new Vector4(cameraTransform.Forward, 0);
                _renderSpaceUniformBlock.Data.LocalToView = cameraTransform.WorldSpaceInverse;
                _renderSpaceUniformBlock.Data.LocalToProjection = cameraTransform.WorldSpaceInverse * cameraProjectionSpace;
                _renderSpaceUniformBlock.Data.LocalToViewRotation = cameraTransform.WorldSpaceInverse.ClearScale().ClearTranslation();
                _renderSpaceUniformBlock.PushToGPU();

                GL.ClearColor(cameraData.ClearColor.X, cameraData.ClearColor.Y, cameraData.ClearColor.Z, cameraData.ClearColor.W);
                GL.Clear(cameraData.ClearMask);

                
                foreach(var shaderRelation in renderGraph)
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


                        foreach (var primitive in materialRelation.Value)
                        {
                            primitive.Draw();
                        }
                    }
                }
            }
        }
    }
}
