using DefaultEcs;
using DefaultEcs.System;
using Framework.ECS.Components.Light;
using Project.ECS.Components;

namespace Project.ECS.Systems
{
    [With(typeof(ReflectionProbeComponent))]
    [With(typeof(ReflectionProbeUpdateComponent))]
    public class ReflectionProbeUpdateSystem : AEntitySetSystem<bool>
    {
        private readonly Entity _worldComponents;

        /// <summary>
        /// 
        /// </summary>
        public ReflectionProbeUpdateSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, in Entity entity)
        {
            ref var probeConfig = ref entity.Get<ReflectionProbeComponent>();
            ref var probeUpdate = ref entity.Get<ReflectionProbeUpdateComponent>();

            probeConfig.HasChanged = true;
        }
    }
}
