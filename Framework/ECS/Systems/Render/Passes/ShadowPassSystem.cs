using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Framebuffer;
using Framework.Assets.Materials;
using Framework.Assets.Shader;
using Framework.Assets.Shader.Block;
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
    public class ShadowPassSystem : RenderPassBaseSystem
    {
        /// <summary>
        /// 
        /// </summary>
        public ShadowPassSystem(World world, Entity worldComponents) : base(world, worldComponents)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected override RenderPassDataComponent ValidatePassData(Entity entity)
        {
            if (!entity.Has<RenderPassDataComponent>())
            {
                var shadowCaster = entity.Get<ShadowCasterComponent>();
                entity.Set(new RenderPassDataComponent()
                {
                    FrameBuffer = new FramebufferAsset("ShadowPass")
                    {
                        Width = shadowCaster.Resolution,
                        Height = shadowCaster.Resolution,

                        DrawMode = DrawBufferMode.None,
                        ReadMode = ReadBufferMode.None,

                        TextureTargets = new List<TextureRenderAsset>()
                        {
                            new TextureRenderAsset("ShadowMap")
                            {
                                Attachment = FramebufferAttachment.DepthAttachment,
                                Width = shadowCaster.Resolution,
                                Height = shadowCaster.Resolution,

                                InternalFormat = PixelInternalFormat.DepthComponent,
                                Format = PixelFormat.DepthComponent,
                                PixelType = PixelType.Float
                            }
                        }
                    }
                });
            }

            return entity.Get<RenderPassDataComponent>();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override ShaderViewSpace CreateViewSpace(Entity entity)
        {
            var shadowCaster = entity.Get<ShadowCasterComponent>();
            var transform = entity.Get<TransformComponent>();
            var projection = Matrix4.CreateOrthographic(100f, 100f, shadowCaster.NearClipping, shadowCaster.FarClipping);

            ShaderBlockSingle<ShaderShadowSpace>.Instance.Data = new ShaderShadowSpace() { ShadowSpace = transform.WorldSpaceInverse * projection };
            ShaderBlockSingle<ShaderShadowSpace>.Instance.PushToGPU();

            return new ShaderViewSpace
            {
                WorldToView = transform.WorldSpaceInverse,
                WorldToViewInverse = transform.WorldSpace,
                WorldToProjection = transform.WorldSpaceInverse * projection,

                WorldToViewRotation = transform.WorldSpaceInverse.ClearScale().ClearTranslation(),
                WorldToProjectionRotation = transform.WorldSpaceInverse.ClearScale().ClearTranslation() * projection,

                ViewPosition = new Vector4(transform.Position, 1),
                ViewDirection = new Vector4(-transform.Forward, 0),
                ViewProjection = projection
            };
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void SelectRenderGraphData(Entity entity, out ShaderProgramAsset shader, out MaterialAsset material, out TransformComponent transform, out VertexPrimitiveAsset verticies)
        {
            verticies = entity.Get<PrimitiveComponent>().Primitive;
            transform = entity.Get<TransformComponent>();
            shader = Defaults.Shader.Program.Shadow;
            material = Defaults.Material.Shadow;
        }
    }
}
