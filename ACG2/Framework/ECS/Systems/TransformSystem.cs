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
            var rootEntities = entities
                .Where(f => f.HasAllComponents(typeof(TransformComponent), typeof(ParentComponent)))
                .Where(f => !f.HasAnyComponents(typeof(ChildComponent)));

            foreach (var entity in rootEntities)
                PassTransformSpace(entity);
        }

        private void PassTransformSpace(Entity entity)
        {
            var transformComponent = entity.GetComponent<TransformComponent>();
            var parentComponent = entity.GetComponent<ParentComponent>();

            foreach(var child in parentComponent.Children)
            {
                if (child.TryGetComponent<TransformComponent>(out var childTransformComponent))
                    childTransformComponent.ParentSpace = transformComponent.WorldSpace;

                if (child.HasAnyComponents(typeof(ParentComponent)))
                    PassTransformSpace(child);
            }    
        }
    }
}
