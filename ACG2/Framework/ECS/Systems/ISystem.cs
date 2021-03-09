using System.Collections.Generic;

namespace Framework.ECS.Systems
{
    public interface ISystem
    {
        void Update(IEnumerable<Entity> entities, IEnumerable<IComponent> sceneComponents);
    }
}
