using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Shader.Block;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Transform;
using Framework.ECS.Systems.Render.OpenGL;

namespace Framework.ECS.Systems.Render.Pipeline
{
    [With(typeof(TransformComponent))]
    [With(typeof(PrimitiveComponent))]
    public class PrimitiveSpaceSystem : AEntitySetSystem<bool>
    {
        private readonly Entity _worldComponents;


        /// <summary>
        /// 
        /// </summary>
        public PrimitiveSpaceSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, in Entity entity)
        {
            ref var transform = ref entity.Get<TransformComponent>();
            ref var primitive = ref entity.Get<PrimitiveComponent>();

            if (primitive.PrimitiveSpaceBlock == null)
                primitive.PrimitiveSpaceBlock = new ShaderPrimitiveSpaceBlock();

            primitive.PrimitiveSpaceBlock.LocalToWorld = transform.WorldSpace;
            primitive.PrimitiveSpaceBlock.LocalToWorldRotation = transform.WorldSpace.ClearScale();

            GPUSync.Push(primitive.PrimitiveSpaceBlock);
        }
    }
}
