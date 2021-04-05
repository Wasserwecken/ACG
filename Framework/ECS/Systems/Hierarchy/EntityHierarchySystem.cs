using Framework.ECS.Components.Relation;
using System.Collections.Generic;
using System.Linq;

namespace Framework.ECS.Systems.Hierarchy
{
    public class EntityHierarchySystem : ISystem
    {
        public void Run(IEnumerable<Entity> entities, IEnumerable<IComponent> sceneComponents)
        {
            var parentEntities = entities.Where(f => f.Components.HasAll(typeof(ParentComponent)));
            foreach (var parentEntity in parentEntities)
                parentEntity.Components.Get<ParentComponent>().Children.Clear();

            var childEntites = entities.Where(f => f.Components.HasAll(typeof(ChildComponent)));
            foreach (var childEntity in childEntites)
            {
                var childComponent = childEntity.Components.Get<ChildComponent>();
                if (childComponent.Parent == null)
                    childEntity.Components.Remove(childComponent);

                else if (childComponent.Parent.Components.TryGet<ParentComponent>(out var parentComponent))
                    parentComponent.Children.Add(childEntity);

                else
                    childComponent.Parent.Components.Add(new ParentComponent() { Children = new List<Entity>() { childEntity } });
            }

            foreach(var parentEntity in parentEntities)
            {
                if (parentEntity.Components.TryGet<ParentComponent>(out var parentComponent))
                    if (parentComponent.Children.Count == 0)
                        parentEntity.Components.Remove(parentComponent);
            }
        }
    }
}
