using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Framebuffer;
using Framework.Assets.Materials;
using Framework.Assets.Shader;
using Framework.Assets.Shader.Block.Data;
using Framework.Assets.Textures;
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
            CreateRenderPassComponents(entity);

            var transform = entity.Get<TransformComponent>();
            var shadowCaster = entity.Get<ShadowCasterComponent>();
            ref var renderView = ref entity.Get<RenderPassDataComponent>();

            renderView.ViewSpace = CreateViewSpace(entity, renderView.Projection);
            renderView.Projection = CreateProjection(entity);
            renderView.WorldSpaceInverse = transform.WorldSpaceInverse;
            
            if (renderView.RenderableCandidates == null)
                renderView.RenderableCandidates = _shadowCandidates;

            if (renderView.FrameBuffer == null)
            {
                renderView.FrameBuffer = new FramebufferAsset(shadowCaster.Resolution, shadowCaster.Resolution);
                renderView.FrameBuffer.TextureTargets.Add(new TextureRenderAsset("ShadowMap", FramebufferAttachment.DepthAttachment, shadowCaster.Resolution, shadowCaster.Resolution));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void CreateRenderPassComponents(Entity entity)
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
