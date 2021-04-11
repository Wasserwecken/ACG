using DefaultEcs;
using DefaultEcs.System;
using Framework.ECS.Components.Relation;
using Framework.ECS.Components.Transform;

namespace Framework.ECS.Systems.Hierarchy
{
    [With(typeof(TransformComponent))]
    [With(typeof(ParentComponent))]
    [Without(typeof(ChildComponent))]
    public class TransformHierarchySystem : AEntitySetSystem<bool>
    {
        /// <summary>
        /// 
        /// </summary>
        public TransformHierarchySystem(World world, Entity worldComponents) : base(world) { }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, in Entity entity)
        {
            if (entity.Has<TransformComponent>() && entity.Has<ParentComponent>())
            {
                var transform = entity.Get<TransformComponent>();
                var parent = entity.Get<ParentComponent>();
             
                foreach (var child in parent.Children)
                {
                    if (child.Has<TransformComponent>())
                        child.Get<TransformComponent>().ParentSpace = transform.WorldSpace;

                    Update(state, child);
                }
            }
        }
    }
}
