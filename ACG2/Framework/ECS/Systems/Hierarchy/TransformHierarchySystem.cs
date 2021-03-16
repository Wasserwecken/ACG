using Framework.ECS.Components.Relation;
using Framework.ECS.Components.Transform;
using System.Collections.Generic;
using System.Linq;

namespace Framework.ECS.Systems
{
    public class TransformHierarchySystem : ISystem
    {
        public void Update(IEnumerable<Entity> entities, IEnumerable<IComponent> sceneComponents)
        {
            var rootEntities = entities.Where(f => f.HasComponent<TransformComponent>() && !f.HasComponent<ChildComponent>());
            foreach (var entity in rootEntities)
                PassTransformSpace(entity);
        }

        private void PassTransformSpace(Entity entity)
        {
            if (entity.TryGetComponent<TransformComponent>(out var transformComponent) &&
                entity.TryGetComponent<ParentComponent>(out var parentComponent))
            {
                foreach(var child in parentComponent.Children)
                {
                    if (child.TryGetComponent<TransformComponent>(out var childTransformComponent))
                        childTransformComponent.ParentSpace = transformComponent.WorldSpace;

                    PassTransformSpace(child);
                }    
            }
        }
    }
}
