using ACG.Framework.Assets;
using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Materials;
using Framework.Assets.Textures;
using Framework.ECS.Components.Scene;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace Framework.ECS.Systems.Render
{
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
            foreach (var buffer in AssetRegister.Framebuffers)
                foreach (var texture in buffer.TextureTargets)
                    _renderTextures.Add(texture);

            var aspect = _worldComponents.Get<AspectRatioComponent>();
            var gridWidth = aspect.Width / 5;
            var gridHeight = gridWidth;

            var shader = Defaults.Shader.Program.FrameBuffer;
            var material = new MaterialAsset("FramebufferDebug") { DepthTest = DepthFunction.Always };

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            Renderer.UseShader(shader);

            for (int i = 0; i < _renderTextures.Count; i++)
            {
                GL.Viewport(
                    i * gridWidth + 10,
                    i * gridHeight + 10,
                    (i + 1) * gridWidth - 10,
                    (i + 1) * gridHeight - 10
                );

                material.SetUniform("BufferMap", _renderTextures[i]);
                Renderer.UseMaterial(material, shader);
                Renderer.Draw(Defaults.Vertex.Mesh.Plane[0]);
            }
        }
    }
}
