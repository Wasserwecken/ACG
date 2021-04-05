using Framework.ECS.Components.Relation;
using Framework.ECS.Components.Transform;
using System.Collections.Generic;
using System.Linq;

namespace Framework.ECS.Systems.Hierarchy
{
    public class TransformHierarchySystem : ISystem
    {
        public void Run(IEnumerable<Entity> entities, IEnumerable<IComponent> sceneComponents)
        {
            var rootEntities = entities.Where(f => f.Components.Has<TransformComponent>() && !f.Components.Has<ChildComponent>());
            foreach (var entity in rootEntities)
                PassTransformSpace(entity);
        }

        private void PassTransformSpace(Entity entity)
        {
            if (entity.Components.TryGet<TransformComponent>(out var transformComponent) &&
                entity.Components.TryGet<ParentComponent>(out var parentComponent))
            {
                foreach(var child in parentComponent.Children)
                {
                    if (child.Components.TryGet<TransformComponent>(out var childTransformComponent))
                        childTransformComponent.ParentSpace = transformComponent.WorldSpace;

                    PassTransformSpace(child);
                }    
            }
        }
    }
}
