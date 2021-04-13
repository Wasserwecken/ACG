﻿using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Framebuffer;
using Framework.Assets.Materials;
using Framework.Assets.Shader;
using Framework.Assets.Shader.Block.Data;
using Framework.Assets.Verticies;
using Framework.ECS.Components.Light;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Transform;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace Framework.ECS.Systems.Render
{
    [With(typeof(TransformComponent))]
    [With(typeof(ShadowCasterComponent))]
    public class RenderPassShadowSystem : AEntitySetSystem<bool>
    {
        private readonly Entity _worldComponents;
        private readonly EntitySet _shadowCandidates;

        /// <summary>
        /// 
        /// </summary>
        public RenderPassShadowSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
            _shadowCandidates = World.GetEntities()
                .With<TransformComponent>()
                .With<PrimitiveComponent>()
                .AsSet();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, in Entity entity)
        {
            ValidateRenderPassComponents(entity);

            var transform = entity.Get<TransformComponent>();
            ref var renderView = ref entity.Get<RenderPassDataComponent>();

            renderView.ViewSpace = CreateViewSpace(entity, renderView.Projection);
            renderView.Projection = CreateProjection(entity);
            renderView.WorldSpaceInverse = transform.WorldSpaceInverse;
            renderView.RenderableCandidates = _shadowCandidates;
            renderView.FrameBuffer = new FramebufferAsset()
            {
                Width = 2048,
                Height = 2048,
                TextureTargets = new List<FrameBufferTextureAsset>()
                {
                    new FrameBufferTextureAsset("")
                    {
                        PixelType = PixelType.Float,
                        Format = PixelFormat.DepthComponent,
                        InternalFormat = PixelInternalFormat.DepthComponent
                    }
                },
                StorageTargets = new List<FramebufferStorageAsset>()
                {
                    new FramebufferStorageAsset("") { DataType = RenderbufferStorage.DepthComponent }
                }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        private void ValidateRenderPassComponents(Entity entity)
        {
            if (!entity.Has<RenderPassDataComponent>())
                entity.Set(new RenderPassDataComponent()
                {
                    Renderables = new List<Entity>(),
                    Graph = new Dictionary<ShaderProgramAsset, Dictionary<MaterialAsset, Dictionary<TransformComponent, List<VertexPrimitiveAsset>>>>(),
                });

            if (!entity.Has<RenderPassShaderComponent>())
                entity.Set(new RenderPassShaderComponent()
                {
                    Shader = Defaults.Shader.Program.MeshUnlit,
                    Material = Defaults.Material.PBR
                });
        }

        /// <summary>
        /// 
        /// </summary>
        private Matrix4 CreateProjection(Entity entity)
        {
            var shadowCaster = entity.Get<ShadowCasterComponent>();
            return Matrix4.CreateOrthographic(10f, 10f, shadowCaster.NearClipping, shadowCaster.FarClipping);
        }

        /// <summary>
        /// 
        /// </summary>
        private ShaderViewSpace CreateViewSpace(Entity entity, Matrix4 projection)
        {
            var transform = entity.Get<TransformComponent>();

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
    }
}
