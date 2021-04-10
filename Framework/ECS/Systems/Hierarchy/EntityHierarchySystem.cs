using DefaultEcs;
using DefaultEcs.System;
using Framework.ECS.Components.Relation;
using System;
using System.Collections.Generic;

namespace Framework.ECS.Systems.Hierarchy
{
    public class EntityHierarchySystem : AEntitySetSystem<bool>
    {
        /// <summary>
        /// 
        /// </summary>
        public EntityHierarchySystem(World world) : base(world) { }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, ReadOnlySpan<Entity> entities)
        {
            var parentEntities = World.GetEntities().With<ParentComponent>().AsSet().GetEntities();
            foreach (var parent in parentEntities)
                parent.Get<ParentComponent>().Children.Clear();

            var childEntites = World.GetEntities().With<ChildComponent>().AsSet().GetEntities();
            foreach (var child in childEntites)
            {
                var childComponent = child.Get<ChildComponent>();
                if (childComponent.Parent == null)
                    child.Remove<ChildComponent>();

                else if (childComponent.Parent.Has<ParentComponent>())
                    childComponent.Parent.Get<ParentComponent>().Children.Add(child);

                else
                    childComponent.Parent.Set(new ParentComponent() { Children = new List<Entity>() { child } });
            }

            foreach (var parent in parentEntities)
                if (parent.Has<ParentComponent>())
                    if (parent.Get<ParentComponent>().Children.Count == 0)
                        parent.Remove<ParentComponent>();
        }
    }
}
