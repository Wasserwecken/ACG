using ACG.Framework.Assets;
using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Materials;
using Framework.Assets.Shader;
using Framework.Assets.Shader.Block;
using Framework.Assets.Shader.Block.Data;
using Framework.Assets.Textures;
using Framework.Assets.Verticies;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Scene;
using Framework.ECS.Components.Transform;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Linq;

namespace Framework.ECS.Systems.Render
{
    [With(typeof(TransformComponent))]
    [With(typeof(PerspectiveCameraComponent))]
    public class ForwardPassSystemOLD : AEntitySetSystem<bool>
    {
        private readonly Entity _worldComponents;
        private EntitySet _graphSet;
        private readonly Dictionary<ShaderProgramAsset,
           Dictionary<MaterialAsset,
                List<PrimitiveComponent>>> _graph;

        /// <summary>
        /// 
        /// </summary>
        public ForwardPassSystemOLD(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
            _graphSet = World.GetEntities().With<TransformComponent>().With<PrimitiveComponent>().AsSet();
            _graph = new Dictionary<ShaderProgramAsset, Dictionary<MaterialAsset, List<PrimitiveComponent>>>();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void PreUpdate(bool state)
        {
            _graph.Clear();

            foreach (var entity in _graphSet.GetEntities())
            {
                var primitive = entity.Get<PrimitiveComponent>();

                var shader = primitive.Shader;
                var material = primitive.Material;

                if (!_graph.ContainsKey(shader))
                    _graph.Add(shader, new Dictionary<MaterialAsset, List<PrimitiveComponent>>());

                if (!_graph[shader].ContainsKey(material))
                    _graph[shader].Add(material, new List<PrimitiveComponent>());

                _graph[shader][material].Add(primitive);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, in Entity entity)
        {
            var cameraData = entity.Get<PerspectiveCameraComponent>();
            var cameraTransform = entity.Get<TransformComponent>();
            var aspectRatio = _worldComponents.Get<AspectRatioComponent>();


            TextureBaseAsset skyboxTexture = Defaults.Texture.SkyboxCoast;
            var projectionSpace = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(cameraData.FieldOfView),
                aspectRatio.Ratio,
                cameraData.NearClipping,
                cameraData.FarClipping
            );

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, aspectRatio.Width, aspectRatio.Height);
            UseCamera(cameraData);

            if (cameraData.ShaderViewSpace == null)
                cameraData.ShaderViewSpace = new ShaderViewSpaceBlock();

            cameraData.ShaderViewSpace.WorldToView = cameraTransform.WorldSpaceInverse;
            cameraData.ShaderViewSpace.WorldToProjection = cameraTransform.WorldSpaceInverse * projectionSpace;
            cameraData.ShaderViewSpace.WorldToViewRotation = cameraTransform.WorldSpaceInverse.ClearScale().ClearTranslation();
            cameraData.ShaderViewSpace.WorldToProjectionRotation = cameraTransform.WorldSpaceInverse.ClearScale().ClearTranslation() * projectionSpace;
            cameraData.ShaderViewSpace.ViewPosition = new Vector4(cameraTransform.Position, 1);
            cameraData.ShaderViewSpace.ViewDirection = new Vector4(cameraTransform.Forward, 0);
            cameraData.ShaderViewSpace.Resolution = new Vector2(aspectRatio.Width, aspectRatio.Height);

            GPUSync.Push(cameraData.ShaderViewSpace);

            foreach (var shaderRelation in _graph)
            {
                UseShader(shaderRelation.Key);
                Renderer.UseShaderBlock(cameraData.ShaderViewSpace, shaderRelation.Key);

                foreach (var materialRelation in shaderRelation.Value)
                {
                    materialRelation.Key.SetUniform(Definitions.Shader.Uniform.ReflectionMap, skyboxTexture);
                    UseMaterial(materialRelation.Key);
                    SetUniforms(materialRelation.Key, shaderRelation.Key);

                    foreach (var primitive in materialRelation.Value)
                    {
                        Renderer.UseShaderBlock(primitive.ShaderSpaceBlock, shaderRelation.Key);
                        Renderer.Draw(primitive.Verticies);
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
            foreach (var binding in shader.BlockBindings.Values)
                GL.BindBufferBase(binding.Target, binding.Layout, binding.Handle);
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

            foreach (var uniformTexture in shader.UniformInfos.Where(f => f.Type == ActiveUniformType.Sampler2D))
            {
                GL.Uniform1(uniformTexture.Layout, uniformTexture.Layout);
                GL.ActiveTexture(TextureUnit.Texture0 + uniformTexture.Layout);

                if (uniformTexture.Name.ToLower().Contains("normal"))
                    GL.BindTexture(Defaults.Texture.Normal.Target, Defaults.Texture.Normal.Handle);
                else
                    GL.BindTexture(Defaults.Texture.White.Target, Defaults.Texture.White.Handle);
            }

            foreach (var uniform in material.UniformTextures)
                if (shader.IdentifierToLayout.TryGetValue(uniform.Key, out var layout))
                {
                    GL.Uniform1(layout, layout);
                    GL.ActiveTexture(TextureUnit.Texture0 + layout);
                    GL.BindTexture(uniform.Value.Target, uniform.Value.Handle);
                }

            foreach (var frameBuffer in AssetRegister.Framebuffers)
                foreach (var renderTexture in frameBuffer.Textures)
                    if (shader.IdentifierToLayout.TryGetValue(renderTexture.Name, out var layout))
                    {
                        GL.Uniform1(layout, layout);
                        GL.ActiveTexture(TextureUnit.Texture0 + layout);
                        GL.BindTexture(renderTexture.Target, renderTexture.Handle);
                    }
        }

        /// <summary>
        /// 
        /// </summary>
        private ShaderPrimitiveSpaceBlock CreatePrimitiveSpace(TransformComponent primitiveTransform, TransformComponent viewTransform, Matrix4 projection)
        {
            return new ShaderPrimitiveSpaceBlock
            {
                LocalToWorld = primitiveTransform.WorldSpace,
                LocalToWorldRotation = primitiveTransform.WorldSpace.ClearScale(),
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
