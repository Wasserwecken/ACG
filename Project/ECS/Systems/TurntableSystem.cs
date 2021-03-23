using Framework;
using Framework.ECS;
using Framework.ECS.Components.Scene;
using Framework.ECS.Components.Transform;
using Framework.ECS.Systems;
using Framework.Extensions;
using OpenTK.Mathematics;
using Project.ECS.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project.ECS.Systems
{
    public class TurntableSystem : ISystem
    {
        /// <summary>
        /// 
        /// </summary>
        public void Run(IEnumerable<Entity> entities, IEnumerable<IComponent> sceneComponents)
        {
            var turntableEntities = entities.Where(f => f.Components.Has<TurntableComponent>());
            var time = sceneComponents.Get<TimeComponent>();

            foreach(var entitity in turntableEntities)
            {
                var turntable = entitity.Components.Get<TurntableComponent>();
                var transform = entitity.Components.Get<TransformComponent>();

                transform.Forward = transform.Forward.Rotate(turntable.Speed * time.DeltaFrame, Vector3.UnitY);
            }
        }
    }
}
