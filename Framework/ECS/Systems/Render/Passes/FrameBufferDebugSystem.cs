using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Materials;
using Framework.Assets.Textures;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Scene;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace Framework.ECS.Systems.Render
{
    [With(typeof(RenderPassDataComponent))]
    public class FrameBufferDebugSystem : AEntitySetSystem<bool>
    {
        protected readonly Entity _worldComponents;
        private List<TextureBaseAsset> _renderTextures;

        /// <summary>
        /// 
        /// </summary>
        public FrameBufferDebugSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
            _renderTextures = new List<TextureBaseAsset>();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void PreUpdate(bool state)
        {
            _renderTextures.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, in Entity entity)
        {
            var passData = entity.Get<RenderPassDataComponent>();
            foreach (var texture in passData.FrameBuffer.TextureTargets)
                _renderTextures.Add(texture);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void PostUpdate(bool state)
        {
            var aspect = _worldComponents.Get<AspectRatioComponent>();
            var gridWidth = aspect.Width / 2;
            var gridHeight = aspect.Height / 2;

            var shader = Defaults.Shader.Program.FrameBuffer;
            var material = new MaterialAsset("FramebufferDebug");

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            Renderer.UseShader(shader);

            for (int i = 0; i < _renderTextures.Count; i++)
            {
                GL.Viewport(
                    i * gridWidth,
                    i * gridHeight,
                    (i + 1) * gridWidth,
                    (i + 1) * gridHeight
                );

                material.SetUniform("BufferMap", _renderTextures[i]);
                Renderer.UseMaterial(material, shader);
                Renderer.Draw(Defaults.Vertex.Mesh.Plane[0]);
            }
        }
    }
}
