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
        public Entity(string name, params IComponent[] components)
        {
            Name = name;
            Components = new List<IComponent>(components);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool HasComponent<TType>()
        {
            foreach (var component in Components)
                if (component is TType)
                    return true;

            return false;
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
