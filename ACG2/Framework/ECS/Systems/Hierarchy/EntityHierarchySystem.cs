using Framework.ECS.Components.Relation;
using System.Collections.Generic;
using System.Linq;

namespace Framework.ECS.Systems
{
    public class EntityHierarchySystem : ISystem
    {
        public void Update(IEnumerable<Entity> entities, IEnumerable<IComponent> sceneComponents)
        {
            var parentEntities = entities.Where(f => f.HasAnyComponents(typeof(ParentComponent)));
            foreach (var parentEntity in parentEntities)
                parentEntity.GetComponent<ParentComponent>().Children.Clear();

            var childEntites = entities.Where(f => f.HasAllComponents(typeof(ChildComponent)));
            foreach (var childEntity in childEntites)
            {
                var childComponent = childEntity.GetComponent<ChildComponent>();
                if (childComponent.Parent == null)
                    childEntity.Components.Remove(childComponent);

                else if (childComponent.Parent.TryGetComponent<ParentComponent>(out var parentComponent))
                    parentComponent.Children.Add(childEntity);

                else
                    childComponent.Parent.Components.Add(new ParentComponent() { Children = new List<Entity>() { childEntity } });
            }

            foreach(var parentEntity in parentEntities)
            {
                if (parentEntity.TryGetComponent<ParentComponent>(out var parentComponent))
                    if (parentComponent.Children.Count == 0)
                        parentEntity.Components.Remove(parentComponent);
            }
        }
    }
}
