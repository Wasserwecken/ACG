using Framework.ECS.Systems;
using System.Collections.Generic;
using System.Linq;

namespace Framework.ECS.Pipeline
{
    public class Pipeline
    {
        public List<IComponent> Components { get; set; }
        public List<ISystem> Systems { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Pipeline()
        {
            Components = new List<IComponent>();
            Systems = new List<ISystem>();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Process(List<Entity> entities, IEnumerable<IComponent> globalComponents)
        {
            var components = Components.Concat(globalComponents);
            foreach (var system in Systems)
                system.Run(entities, components);
        }
    }
}
