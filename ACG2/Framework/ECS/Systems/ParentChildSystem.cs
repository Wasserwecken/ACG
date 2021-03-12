using Framework.ECS.Components.Relation;
using System.Collections.Generic;
using System.Linq;

namespace Framework.ECS.Systems
{
    public class ParentChildSystem : ISystem
    {
        public void Update(IEnumerable<Entity> entities, IEnumerable<IComponent> sceneComponents)
        {
            var parents = entities.Where(f => f.HasAllComponents(typeof(ChildComponent)));
            foreach (var parent in parents)
                parent.GetComponent<ChildComponent>().Children.Clear();

            var children = entities.Where(f => f.HasAllComponents(typeof(ParentComponent)));
            foreach (var child in children)
            {
                child.TryGetComponent<ParentComponent>(out var parentComponent);
                if (parentComponent.Parent.TryGetComponent<ChildComponent>(out var childComponent))
                    childComponent.Children.Add(child);
                else
                    parentComponent.Parent.Components.Add(new ChildComponent() { Children = new List<Entity>() { child } });
            }

            foreach(var parent in parents)
            {
                parent.TryGetComponent<ChildComponent>(out var childComponent);
                if (childComponent.Children.Count == 0)
                    parent.Components.Remove(childComponent);
            }

        }
    }
}
