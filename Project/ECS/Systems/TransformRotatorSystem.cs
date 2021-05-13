using DefaultEcs;
using DefaultEcs.System;
using Framework.ECS.Components.Scene;
using Framework.ECS.Components.Transform;
using Framework.Extensions;
using OpenTK.Mathematics;
using Project.ECS.Components;

namespace Project.ECS.Systems
{
    [With(typeof(TransformComponent))]
    [With(typeof(TransformRotatorComponent))]
    class TransformRotatorSystem : AEntitySetSystem<bool>
    {
        private readonly Entity _worldComponents;

        /// <summary>
        /// 
        /// </summary>
        public TransformRotatorSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, in Entity entity)
        {
            ref var transform = ref entity.Get<TransformComponent>();
            var time = _worldComponents.Get<TimeComponent>();
            var rotator = entity.Get<TransformRotatorComponent>();

            transform.Forward = transform.Forward.Rotate(rotator.Speed * time.DeltaFrame, Vector3.UnitY);
        }
    }
}
