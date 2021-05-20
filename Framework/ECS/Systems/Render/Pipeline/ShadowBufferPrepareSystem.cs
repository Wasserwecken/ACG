using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Shader.Block;
using Framework.Assets.Textures;
using Framework.ECS.Components.Scene;

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
            if (component.TextureAtlas == null || component.TextureAtlas.Resolution != component.Size)
                component.TextureAtlas = new TextureSpace(component.Size);
            component.TextureAtlas.Clear();

            component.DirectionalBlock.Shadows = new ShaderDirectionalShadowBlock.ShaderDirectionalShadow[0];
            component.PointBlock.Shadows = new ShaderPointShadowBlock.ShaderPointShadow[0];
            component.SpotBlock.Shadows = new ShaderSpotShadowBlock.ShaderSpotShadow[0];

            component.FramebufferBuffer.Width = component.Size;
            component.FramebufferBuffer.Height = component.Size;
        }
    }
}
