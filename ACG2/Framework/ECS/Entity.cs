using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Framework.ECS
{
    [DebuggerDisplay("Name: {Name}, Components: {Components.Count}")]
    public class Entity
    {
        public string Name { get; set; }
        public List<IComponent> Components { get; set; }

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
            Components = new List<IComponent>();
        }

        /// <summary>
        /// 
        /// </summary>
        public bool TryGetComponent<TComponentType>(out TComponentType component) where TComponentType : IComponent
        {
            foreach (var attachedComponent in Components)
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
        public TType GetComponent<TType>() where TType : IComponent
        {
            foreach (var component in Components)
                if (component is TType resultComponent)
                    return resultComponent;

            return default;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool HasAllComponents(params Type[] requieredTypes)
        {
            var componentTypes = new List<Type>();
            foreach (var component in Components)
                componentTypes.Add(component.GetType());

            foreach (var type in requieredTypes)
                if (!componentTypes.Contains(type))
                    return false;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool HasAnyComponents(params Type[] requieredTypes)
        {
            var componentTypes = new List<Type>();
            foreach (var component in Components)
                componentTypes.Add(component.GetType());

            foreach (var type in requieredTypes)
                if (componentTypes.Contains(type))
                    return true;

            return false;
        }
    }
}
