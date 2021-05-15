using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Shader.Block;
using Framework.ECS.Components.Scene;

namespace Framework.ECS.Systems.Render.Pipeline
{
    public class ReflectionBufferPrepareSystem : AComponentSystem<bool, ReflectionBufferComponent>
    {
        /// <summary>
        /// 
        /// </summary>
        public ReflectionBufferPrepareSystem(World world, Entity worldComponents) : base(world)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, ref ReflectionBufferComponent component)
        {
            component.TextureAtlas.Clear();
            component.ReflectionBlock.Probes = new ShaderReflectionBlock.ShaderReflectionProbe[0];
        }
    }
}
