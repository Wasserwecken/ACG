using DefaultEcs;
using DefaultEcs.System;
using Framework.ECS.Components.Relation;
using System;
using System.Collections.Generic;

namespace Framework.ECS.Systems.Hierarchy
{
    public class EntityHierarchySystem : AEntitySetSystem<bool>
    {
        private readonly EntitySet _parentEntities;
        private readonly EntitySet _childEntities;

        /// <summary>
        /// 
        /// </summary>
        public EntityHierarchySystem(World world, Entity worldComponents) : base(world)
        {
            _parentEntities = World.GetEntities().With<ParentComponent>().AsSet();
            _childEntities = World.GetEntities().With<ChildComponent>().AsSet();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, ReadOnlySpan<Entity> entities)
        {
            var parentEntities = _parentEntities.GetEntities();
            foreach (var parent in parentEntities)
                parent.Get<ParentComponent>().Children.Clear();

            foreach (var child in _childEntities.GetEntities())
            {
                var childComponent = child.Get<ChildComponent>();
                if (childComponent.Parent == null)
                    child.Remove<ChildComponent>();

                else if (childComponent.Parent.Has<ParentComponent>())
                    childComponent.Parent.Get<ParentComponent>().Children.Add(child);

                else
                    childComponent.Parent.Set(new ParentComponent(child));
            }

            foreach (var parent in parentEntities)
                if (parent.Has<ParentComponent>())
                    if (parent.Get<ParentComponent>().Children.Count == 0)
                        parent.Remove<ParentComponent>();
        }
    }
}
