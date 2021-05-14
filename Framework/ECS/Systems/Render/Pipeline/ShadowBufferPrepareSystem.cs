using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Shader.Block;
using Framework.ECS.Components.Light;
using Framework.ECS.Systems.Render.OpenGL;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.ECS.Systems.Render.Pipeline
{
    public class ShadowBufferPrepareSystem : AComponentSystem<bool, ShadowBufferComponent>
    {
        /// <summary>
        /// 
        /// </summary>
        public ShadowBufferPrepareSystem(World world, Entity worldComponents) : base(world)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, ref ShadowBufferComponent component)
        {
            component.TextureAtlas.Clear();

            component.DirectionalBlock.Shadows = new ShaderDirectionalShadowBlock.ShaderDirectionalShadow[0];
            component.PointBlock.Shadows = new ShaderPointShadowBlock.ShaderPointShadow[0];
            component.SpotBlock.Shadows = new ShaderSpotShadowBlock.ShaderSpotShadow[0];

            Renderer.UseFrameBuffer(component.FramebufferBuffer);
            GL.ClearColor(component.FramebufferBuffer.ClearColor);
            GL.Clear(component.FramebufferBuffer.ClearMask);
        }
    }
}
