using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public class Entity
    {
        public string Name { get; set; }
        private readonly List<IEntityComponent> _components;

        /// <summary>
        /// 
        /// </summary>
        public Entity() : this(string.Empty, default) { }

        /// <summary>
        /// 
        /// </summary>
        public Entity(string name) : this(name, default) { }

        /// <summary>
        /// 
        /// </summary>
        public Entity(string name, IEnumerable<IEntityComponent> initialComponents)
        {
            Name = name;

            if (initialComponents != null)
                _components = new List<IEntityComponent>(initialComponents);
            else
                _components = new List<IEntityComponent>();
        }

        /// <summary>
        /// 
        /// </summary>
        public void AddComponent(IEntityComponent component)
        {
            _components.Add(component);
        }

        /// <summary>
        /// 
        /// </summary>
        public List<IEntityComponent> GetComponents<TComponentType>() where TComponentType : IEntityComponent
        {
            var results = new List<IEntityComponent>();

            foreach (var component in _components)
                if (component is TComponentType)
                    results.Add(component);

            return results;
        }
    }
}
