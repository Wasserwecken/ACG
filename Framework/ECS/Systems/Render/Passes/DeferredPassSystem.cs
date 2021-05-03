using DefaultEcs;
using Framework.Assets.Framebuffer;
using Framework.Assets.Materials;
using Framework.Assets.Shader;
using Framework.Assets.Shader.Block.Data;
using Framework.Assets.Textures;
using Framework.Assets.Verticies;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Scene;
using Framework.ECS.Components.Transform;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.ECS.Systems.Render
{
    public class DeferredPassSystem : RenderPassBaseSystem
    {
        /// <summary>
        /// 
        /// </summary>
        public DeferredPassSystem(World world, Entity worldComponents) : base(world, worldComponents)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        protected override RenderPassDataComponent ValidatePassData(Entity entity)
        {
            if (!entity.Has<RenderPassDataComponent>())
                entity.Set(new RenderPassDataComponent());

            ref var renderPassData = ref entity.Get<RenderPassDataComponent>();
            var aspect = _worldComponents.Get<AspectRatioComponent>();

            //if (renderPassData.FrameBuffer == null)
            //{
            //    renderPassData.FrameBuffer = new FramebufferAsset(aspect.Width, aspect.Height)
            //    {
            //        DrawMode = DrawBufferMode.ColorAttachment0 | DrawBufferMode.ColorAttachment1 | DrawBufferMode.ColorAttachment2 | DrawBufferMode.ColorAttachment3,
            //        ReadMode = ReadBufferMode.Front
            //    };

            //    renderPassData.FrameBuffer.TextureTargets.Add(new TextureRenderAsset("DeferredPosition", FramebufferAttachment.ColorAttachment0, aspect.Width, aspect.Height));
            //    renderPassData.FrameBuffer.TextureTargets.Add(new TextureRenderAsset("DeferredNormal", FramebufferAttachment.ColorAttachment1, aspect.Width, aspect.Height));
            //    renderPassData.FrameBuffer.TextureTargets.Add(new TextureRenderAsset("DeferredAlbedo", FramebufferAttachment.ColorAttachment2, aspect.Width, aspect.Height));
            //    renderPassData.FrameBuffer.TextureTargets.Add(new TextureRenderAsset("DeferredMREO", FramebufferAttachment.ColorAttachment3, aspect.Width, aspect.Height));
            //    renderPassData.FrameBuffer.StorageTargets.Add(new FramebufferStorageAsset("DeferredDepth"));
            //}

            return renderPassData;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override ShaderViewSpace CreateViewSpace(Entity entity)
        {
            var aspect = _worldComponents.Get<AspectRatioComponent>();
            var camera = entity.Get<PerspectiveCameraComponent>();
            var transform = entity.Get<TransformComponent>();

            var projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(camera.FieldOfView),
                aspect.Ratio,
                camera.NearClipping,
                camera.FarClipping
            );

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
        protected override void SelectRenderGraphData(Entity entity, out ShaderProgramAsset shader, out MaterialAsset material, out TransformComponent transform, out VertexPrimitiveAsset verticies)
        {
            shader = entity.Get<PrimitiveComponent>().Shader;
            material = entity.Get<PrimitiveComponent>().Material;
            verticies = entity.Get<PrimitiveComponent>().Primitive;
            transform = entity.Get<TransformComponent>();
        }
    }
}
