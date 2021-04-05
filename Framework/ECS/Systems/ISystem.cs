using System.Collections.Generic;

namespace Framework.ECS.Systems
{
    public interface ISystem
    {
        void Run(IEnumerable<Entity> entities, IEnumerable<IComponent> globalComponents);
    }
}
