﻿using ACG.Framework.Assets;
using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Materials;
using Framework.Assets.Textures;
using Framework.ECS.Components.Scene;
using Framework.ECS.Systems.Render.OpenGL;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Collections.Generic;

namespace Framework.ECS.Systems.Render
{
    public class FrameBufferDebugSystem : AEntitySetSystem<bool>
    {
        protected readonly Entity _worldComponents;
        private List<TextureBaseAsset> _renderTextures;
        private bool _isVisible;

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
            var input = _worldComponents.Get<InputComponent>();
            if (input.Keyboard.IsKeyReleased(Keys.F9))
                _isVisible = !_isVisible;

            if (!_isVisible)
                return;

            _renderTextures.Clear();
            foreach (var buffer in AssetRegister.Framebuffers)
                foreach (var texture in buffer.Textures)
                    _renderTextures.Add(texture);

            var aspect = _worldComponents.Get<AspectRatioComponent>();
            var tileCount = 8;
            var gridWidth = aspect.Width / tileCount;
            var gridHeight = gridWidth;

            var shader = Defaults.Shader.Program.FrameBuffer;
            var material = new MaterialAsset("FramebufferDebug") { DepthTest = DepthFunction.Always };

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            Renderer.Use(shader);

            for (int i = 0; i < _renderTextures.Count; i++)
            {
                var margin = 10;
                var x = i % tileCount;
                var y = i == 0 ? 0 : i / tileCount;
                GL.Viewport(
                    x * gridWidth + margin,
                    y * gridHeight + margin,
                    (x + 1) + gridWidth - margin,
                    (y + 1) + gridHeight - margin
                );

                material.SetUniform("BufferMap", _renderTextures[i]);
                Renderer.Use(material, shader);
                Renderer.Draw(Defaults.Vertex.Mesh.Plane[0]);
            }
        }
    }
}
