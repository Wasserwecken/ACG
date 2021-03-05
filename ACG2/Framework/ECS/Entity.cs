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
        public bool HasComponents(params Type[] componentTypes)
        {
            foreach (var component in Components)
                foreach (var type in componentTypes)
                    if (component.GetType() != type)
                        return false;

            return true;
        }
    }
}
