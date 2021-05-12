using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Shader.Block;
using Framework.Assets.Shader.Block.Data;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Transform;
using Framework.ECS.Systems.Render;
using OpenTK.Graphics.OpenGL;
using System;

namespace ACG.Framework.ECS.Systems.Hierarchy
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

            if (primitive.ShaderSpaceBlock == null)
                primitive.ShaderSpaceBlock = new ShaderPrimitiveSpaceBlock();

            primitive.ShaderSpaceBlock.LocalToWorld = transform.WorldSpace;
            primitive.ShaderSpaceBlock.LocalToWorldRotation = transform.WorldSpace.ClearScale();

            GPUSync.Push(primitive.ShaderSpaceBlock);
        }
    }
}
