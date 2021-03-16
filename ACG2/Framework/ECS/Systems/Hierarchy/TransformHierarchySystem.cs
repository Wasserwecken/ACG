﻿using Framework.ECS.Components.Relation;
using Framework.ECS.Components.Transform;
using System.Collections.Generic;
using System.Linq;

namespace Framework.ECS.Systems.Hierarchy
{
    public class TransformHierarchySystem : ISystem
    {
        public void Run(IEnumerable<Entity> entities, IEnumerable<IComponent> sceneComponents)
        {
            var rootEntities = entities.Where(f => f.HasComponent<TransformComponent>() && !f.HasComponent<ChildComponent>());
            foreach (var entity in rootEntities)
                PassTransformSpace(entity);
        }

        private void PassTransformSpace(Entity entity)
        {
            if (entity.Components.Has<TransformComponent>(out var transformComponent) &&
                entity.Components.Has<ParentComponent>(out var parentComponent))
            {
                foreach(var child in parentComponent.Children)
                {
                    if (child.Components.Has<TransformComponent>(out var childTransformComponent))
                        childTransformComponent.ParentSpace = transformComponent.WorldSpace;

                    PassTransformSpace(child);
                }    
            }
        }
    }
}