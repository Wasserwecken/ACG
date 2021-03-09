using Framework.ECS.Components.Relation;
using Framework.ECS.Components.Transform;
using System.Collections.Generic;
using System.Linq;

namespace Framework.ECS.Systems
{
    public class TransformSystem : ISystem
    {
        public void Update(IEnumerable<Entity> entities, IEnumerable<IComponent> sceneComponents)
        {
            var relevantEntities = entities
                .Where(f => f.HasAllComponents(typeof(TransformComponent), typeof(ChildComponent)))
                .Where(f => !f.HasAnyComponents(typeof(ParentComponent)));

            foreach (var entity in relevantEntities)
                PassTransformSpace(entity);
        }

        private void PassTransformSpace(Entity entity)
        {
            entity.TryGetComponent<TransformComponent>(out var parentTransform);
            entity.TryGetComponent<ChildComponent>(out var children);

            foreach(var child in children.Children)
            {
                if (child.TryGetComponent<TransformComponent>(out var childTransform))
                    childTransform.ParentSpace = parentTransform.WorldSpace;

                if (child.TryGetComponent<ChildComponent>(out var subChildren))
                    foreach (var subChild in subChildren.Children)
                        PassTransformSpace(subChild);
            }    
        }
    }
}
