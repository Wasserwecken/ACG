using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Shader.Block;
using Framework.Assets.Shader.Block.Data;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Transform;
using OpenTK.Graphics.OpenGL;

namespace ACG.Framework.ECS.Systems.Hierarchy
{
    [With(typeof(TransformComponent))]
    [With(typeof(PrimitiveComponent))]
    public class PrimitiveSpaceSystem : AEntitySetSystem<bool>
    {
        /// <summary>
        /// 
        /// </summary>
        public PrimitiveSpaceSystem(World world, Entity worldComponents) : base(world) { }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, in Entity entity)
        {
            ref var transform = ref entity.Get<TransformComponent>();
            ref var primitive = ref entity.Get<PrimitiveComponent>();

            if (primitive.ShaderSpace == null)
                primitive.ShaderSpace = new ShaderBlockSingle<ShaderPrimitiveSpace>(false, BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);

            primitive.ShaderSpace.Data = new ShaderPrimitiveSpace
            {
                LocalToWorld = transform.WorldSpace,
                LocalToWorldRotation = transform.WorldSpace.ClearScale()
            };
            primitive.ShaderSpace.PushToGPU();
        }
    }
}
