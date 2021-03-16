using Framework.ECS.Components.Scene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.ECS.Systems
{
    public class TextureSyncSystem : ISystem
    {
        /// <summary>
        /// 
        /// </summary>
        public void Update(IEnumerable<Entity> entities, IEnumerable<IComponent> sceneComponents)
        {
            var renderGraphComponent = sceneComponents.First(f => f is RenderDataComponent) as RenderDataComponent;
        }
    }
}
