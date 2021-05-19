using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Shader.Block;
using Framework.ECS.Components.Scene;

namespace Framework.ECS.Systems.Render.Pipeline
{
    public class ReflectionForwardPassSystem : AComponentSystem<bool, ReflectionBufferComponent>
    {
        private readonly Entity _worldComponents;
        private readonly ShaderViewSpaceBlock _viewBlock;

        /// <summary>
        /// 
        /// </summary>
        public ReflectionForwardPassSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
            _viewBlock = new ShaderViewSpaceBlock();
        }
    }
}
