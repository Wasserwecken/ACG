using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Framebuffer;
using Framework.Assets.Materials;
using Framework.Assets.Shader;
using Framework.Assets.Shader.Block;
using Framework.Assets.Shader.Block.Data;
using Framework.Assets.Verticies;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Transform;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Linq;

namespace Framework.ECS.Systems.Render
{
    public abstract class RenderPassBaseSystem : AEntitySetSystem<bool>
    {
        protected readonly Entity _worldComponents;
        protected readonly EntitySet _renderCandidates;
        protected readonly ShaderBlockSingle<ShaderViewSpace> _viewSpaceBlock;
        protected readonly ShaderBlockSingle<ShaderPrimitiveSpace> _primitiveSpaceBlock;
        protected readonly Dictionary<ShaderProgramAsset, Dictionary<MaterialAsset, Dictionary<TransformComponent, List<VertexPrimitiveAsset>>>> _renderGraph;


        /// <summary>
        /// 
        /// </summary>
        public RenderPassBaseSystem(World world, Entity worldComponents) : base(world)
        {
            _renderCandidates = SelectRenderCandidates();
            _worldComponents = worldComponents;

            _viewSpaceBlock = new ShaderBlockSingle<ShaderViewSpace>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
            _primitiveSpaceBlock = new ShaderBlockSingle<ShaderPrimitiveSpace>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
            _renderGraph = new Dictionary<ShaderProgramAsset, Dictionary<MaterialAsset, Dictionary<TransformComponent, List<VertexPrimitiveAsset>>>>();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, in Entity entity)
        {
            // DATA PREPERATION
            var passData = ValidatePassData(entity);
            var viewSpace = CreateViewSpace(entity);

            // BUILD GRAPH GRAPH
            _renderGraph.Clear();
            foreach (ref readonly var canidate in _renderCandidates.GetEntities())
                if (!CullCandidate(canidate, viewSpace))
                    AddToGraph(canidate);

            // PREPARE FRAMEBUFFER
            UseFrameBuffer(passData.FrameBuffer);
            _viewSpaceBlock.Data = viewSpace;
            _viewSpaceBlock.PushToGPU();

            // DRAW RENDER GAPH
            foreach (var shaderRelation in _renderGraph)
            {
                UseShader(shaderRelation.Key);
                foreach (var materialRelation in shaderRelation.Value)
                {
                    UseMaterial(materialRelation.Key, shaderRelation.Key);
                    foreach (var transformRelation in materialRelation.Value)
                    {
                        _primitiveSpaceBlock.Data = CreatePrimitiveSpace(transformRelation.Key, viewSpace);
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
        protected virtual EntitySet SelectRenderCandidates() =>
            World.GetEntities()
                .With<TransformComponent>()
                .With<PrimitiveComponent>()
                .AsSet();

        /// <summary>
        /// 
        /// </summary>
        protected abstract RenderPassDataComponent ValidatePassData(Entity entity);

        /// <summary>
        /// 
        /// </summary>
        protected abstract ShaderViewSpace CreateViewSpace(Entity entity);

        /// <summary>
        /// 
        /// </summary>
        protected virtual bool CullCandidate(Entity candidate, ShaderViewSpace viewSpace) => false;

        /// <summary>
        /// 
        /// </summary>
        protected abstract void SelectRenderGraphData(Entity entity, out ShaderProgramAsset shader, out MaterialAsset material, out TransformComponent transform, out VertexPrimitiveAsset verticies);

        /// <summary>
        /// 
        /// </summary>
        protected virtual void AddToGraph(Entity entity)
        {
            SelectRenderGraphData(entity,
                out var shader,
                out var material,
                out var transform,
                out var verticies
            );

            if (!_renderGraph.ContainsKey(shader))
                _renderGraph.Add(shader, new Dictionary<MaterialAsset, Dictionary<TransformComponent, List<VertexPrimitiveAsset>>>());

            if (!_renderGraph[shader].ContainsKey(material))
                _renderGraph[shader].Add(material, new Dictionary<TransformComponent, List<VertexPrimitiveAsset>>());

            if (!_renderGraph[shader][material].ContainsKey(transform))
                _renderGraph[shader][material].Add(transform, new List<VertexPrimitiveAsset>());

            _renderGraph[shader][material][transform].Add(verticies);
        }

        /// <summary>
        /// 
        /// </summary>
        private void UseFrameBuffer(FramebufferAsset framebuffer)
        {
            if (framebuffer.Handle <= 0)
            {
                framebuffer.Handle = GL.GenFramebuffer();
                GL.BindFramebuffer(framebuffer.Target, framebuffer.Handle);

                foreach (var texture in framebuffer.TextureTargets)
                    GL.FramebufferTexture2D(framebuffer.Target, texture.Attachment, texture.Target, texture.Handle, 0);

                GL.DrawBuffer(framebuffer.DrawMode);
                GL.ReadBuffer(framebuffer.ReadMode);

                GL.BindFramebuffer(framebuffer.Target, 0);
            }

            GL.Viewport(0, 0, framebuffer.Width, framebuffer.Height);
            GL.BindFramebuffer(framebuffer.Target, framebuffer.Handle);
            GL.ClearColor(framebuffer.ClearColor);
            GL.Clear(framebuffer.ClearMask);
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
        private void UseMaterial(MaterialAsset material, ShaderProgramAsset shader)
        {
            // MATERIAL SETTINGS
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

            // MATERIAL UNIFORMS
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
        protected virtual ShaderPrimitiveSpace CreatePrimitiveSpace(TransformComponent primitiveTransform, ShaderViewSpace viewSpace)
        {
            return new ShaderPrimitiveSpace
            {
                LocalToWorld = primitiveTransform.WorldSpace,
                LocalToView = primitiveTransform.WorldSpace * viewSpace.WorldToViewInverse,
                LocalToProjection = primitiveTransform.WorldSpace * viewSpace.WorldToViewInverse * viewSpace.ViewProjection,

                LocalToWorldRotation = primitiveTransform.WorldSpace.ClearScale(),
                LocalToViewRotation = (primitiveTransform.WorldSpace * viewSpace.WorldToViewInverse).ClearScale().ClearTranslation(),
                LocalToProjectionRotation = (primitiveTransform.WorldSpace * viewSpace.WorldToViewInverse).ClearScale().ClearTranslation() * viewSpace.ViewProjection,
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
