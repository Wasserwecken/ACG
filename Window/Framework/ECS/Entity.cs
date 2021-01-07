using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Framework
{
    [DebuggerDisplay("Name: {Name}, Components: {_components.Count}")]
    public class Entity
    {
        public string Name { get; set; }
        private readonly List<IEntityComponent> _components;
        private readonly List<Type> _componentTypes;

        /// <summary>
        /// 
        /// </summary>
        public Entity() : this(string.Empty) { }

        /// <summary>
        /// 
        /// </summary>
        public Entity(string name)
        {
            Name = name;
            _components = new List<IEntityComponent>();
            _componentTypes = new List<Type>();
        }

        /// <summary>
        /// 
        /// </summary>
        public void AddComponent(IEntityComponent component)
        {
            _components.Add(component);

            var type = component.GetType();
            if (!_componentTypes.Contains(type))
                _componentTypes.Add(type);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool TryGetComponent<TComponentType>(out TComponentType component) where TComponentType : IEntityComponent
        {
            foreach (var attachedComponent in _components)
            {
                if (attachedComponent is TComponentType castedComponent)
                {
                    component = castedComponent;
                    return true;
                }
            }

            component = default;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<TComponentType> GetComponents<TComponentType>() where TComponentType : IEntityComponent
        {
            var results = new List<TComponentType>();

            foreach (var component in _components)
                if (component is TComponentType)
                    results.Add((TComponentType)component);

            return results;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool HasComponents(params Type[] componentTypes)
        {
            foreach (var type in componentTypes)
                if (!_componentTypes.Contains(type))
                    return false;

            return true;
        }
    }
}
