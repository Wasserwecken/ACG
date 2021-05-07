﻿using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Materials;
using Framework.Assets.Shader;
using Framework.Assets.Shader.Block;
using Framework.Assets.Shader.Block.Data;
using Framework.Assets.Verticies;
using Framework.ECS.Components.Light;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Transform;
using Framework.ECS.Systems.Render;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace Framework.ECS.Systems.RenderPipeline
{
    [With(typeof(TransformComponent))]
    [With(typeof(PointLightComponent))]
    [With(typeof(PointShadowComponent))]
    public class PointShadowPassSystem : AEntitySetSystem<bool>
    {
        private readonly Dictionary<ShaderProgramAsset, Dictionary<MaterialAsset, Dictionary<TransformComponent, List<VertexPrimitiveAsset>>>> _renderGraph;
        private readonly Entity _worldComponents;
        protected readonly EntitySet _renderCandidates;
        private readonly ShaderBlockArray<ShaderPointLight> _pointInfoBlock;

        /// <summary>
        /// 
        /// </summary>
        public PointShadowPassSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
            _renderGraph = new Dictionary<ShaderProgramAsset, Dictionary<MaterialAsset, Dictionary<TransformComponent, List<VertexPrimitiveAsset>>>>();
            _pointInfoBlock = new ShaderBlockArray<ShaderPointLight>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
            _renderCandidates = World.GetEntities()
                .With<TransformComponent>()
                .With<PrimitiveComponent>()
                .AsSet();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, ReadOnlySpan<Entity> entities)
        {
            // GLOBAL PREPERATION
            ref var shaderInfo = ref _worldComponents.Get<PointLightCollectionComponent>();
            shaderInfo.ShadowSpacer.Clear();

            Renderer.UseFrameBuffer(shaderInfo.ShadowBuffer);
            GL.ClearColor(shaderInfo.ShadowBuffer.ClearColor);
            GL.Clear(shaderInfo.ShadowBuffer.ClearMask);

            foreach (ref readonly var entity in entities)
            {
                // DATA COLLECTION
                var transform = entity.Get<TransformComponent>();
                var lightConfig = entity.Get<PointLightComponent>();
                var shadowConfig = entity.Get<PointShadowComponent>();
                var viewSpaces = CreateViewSpace(shadowConfig, lightConfig, transform);

                if (shadowConfig.Strength > float.Epsilon)
                {
                    // DATA PREPERATION
                    shaderInfo.ShadowSpacer.Add(shadowConfig.Resolution, out var shadowMapSpace);
                    shaderInfo.Data[lightConfig.InfoId].ShadowArea = new Vector4(shadowMapSpace, shadowMapSpace.Z);
                    shaderInfo.Data[lightConfig.InfoId].ShadowStrength = new Vector4(shadowConfig.Strength, shadowConfig.NearClipping, 0f, 0f);

                    // BUILD RENDER GRAPH
                    _renderGraph.Clear();
                    foreach (ref readonly var canidate in _renderCandidates.GetEntities())
                        AddToGraph(
                            Defaults.Shader.Program.Shadow,
                            Defaults.Material.Shadow,
                            canidate.Get<TransformComponent>(),
                            canidate.Get<PrimitiveComponent>().Primitive
                        );

                    for(int i = 0; i < viewSpaces.Length; i++)
                    {
                        // VIEWPORT PREPERATION
                        var viewPort = shadowMapSpace * new Vector3(
                            shaderInfo.ShadowBuffer.Width,
                            shaderInfo.ShadowBuffer.Height,
                            shaderInfo.ShadowBuffer.Width);
                        var x = (int)viewPort.X + (i % 3) * ((int)viewPort.Z / 3);
                        var y = (int)viewPort.Y + (i < 3 ? 0 : 1) * ((int)viewPort.Z / 2);
                        GL.Viewport(x, y, (int)viewPort.Z / 3, (int)viewPort.Z / 2);

                        // DRAW RENDER GRAPH
                        ShaderBlockSingle<ShaderViewSpace>.Instance.Data = viewSpaces[i];
                        ShaderBlockSingle<ShaderViewSpace>.Instance.PushToGPU();

                        foreach (var shaderRelation in _renderGraph)
                        {
                            Renderer.UseShader(shaderRelation.Key);
                            foreach (var materialRelation in shaderRelation.Value)
                            {
                                Renderer.UseMaterial(materialRelation.Key, shaderRelation.Key);
                                foreach (var transformRelation in materialRelation.Value)
                                {
                                    ShaderBlockSingle<ShaderPrimitiveSpace>.Instance.Data = Renderer.CreatePrimitiveSpace(transformRelation.Key, viewSpaces[i]);
                                    ShaderBlockSingle<ShaderPrimitiveSpace>.Instance.PushToGPU();

                                    foreach (var primitive in transformRelation.Value)
                                        Renderer.Draw(primitive);
                                }
                            }
                        }

                    }
                }
            }

            // PUSH DATA TO GPU
            _pointInfoBlock.Data = shaderInfo.Data;
            _pointInfoBlock.PushToGPU();
        }

        /// <summary>
        /// 
        /// </summary>
        private ShaderViewSpace[] CreateViewSpace(PointShadowComponent shadowConfig, PointLightComponent lightConfig, TransformComponent transform)
        {
            var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90f), 1f, shadowConfig.NearClipping, lightConfig.Range);
            var orientations = new Matrix4[]
            {
                Matrix4.LookAt(transform.Position, transform.Position + Vector3.UnitZ, Vector3.UnitY),
                Matrix4.LookAt(transform.Position, transform.Position + Vector3.UnitY, Vector3.UnitX),
                Matrix4.LookAt(transform.Position, transform.Position + Vector3.UnitX, Vector3.UnitY),
                Matrix4.LookAt(transform.Position, transform.Position + -Vector3.UnitZ, Vector3.UnitY),
                Matrix4.LookAt(transform.Position, transform.Position + -Vector3.UnitY, -Vector3.UnitX),
                Matrix4.LookAt(transform.Position, transform.Position + -Vector3.UnitX, Vector3.UnitY)
            };

            var result = new ShaderViewSpace[orientations.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = new ShaderViewSpace
                {
                    WorldToView = orientations[i],
                    WorldToProjection = orientations[i] * projection,

                    WorldToViewRotation = orientations[i].ClearScale().ClearTranslation(),
                    WorldToProjectionRotation = orientations[i].ClearScale().ClearTranslation() * projection,

                    ViewPosition = new Vector4(transform.Position, 1),
                    ViewDirection = new Vector4(orientations[i].Row2),
                    ViewProjection = projection
                };

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void AddToGraph(ShaderProgramAsset shader, MaterialAsset material, TransformComponent transform, VertexPrimitiveAsset verticies)
        {
            if (!_renderGraph.ContainsKey(shader))
                _renderGraph.Add(shader, new Dictionary<MaterialAsset, Dictionary<TransformComponent, List<VertexPrimitiveAsset>>>());

            if (!_renderGraph[shader].ContainsKey(material))
                _renderGraph[shader].Add(material, new Dictionary<TransformComponent, List<VertexPrimitiveAsset>>());

            if (!_renderGraph[shader][material].ContainsKey(transform))
                _renderGraph[shader][material].Add(transform, new List<VertexPrimitiveAsset>());

            _renderGraph[shader][material][transform].Add(verticies);
        }
    }
}
