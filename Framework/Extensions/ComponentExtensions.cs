using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Framework.ECS;
using OpenTK.Mathematics;

namespace Framework
{
    public static class ComponentExtensions
    {    
        /// <summary>
        /// 
        /// </summary>
        public static bool Has<TComponent>(this IEnumerable<IComponent> components) where TComponent : IComponent
        {
            foreach (var component in components)
                if (component is TComponent)
                    return true;

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool HasAll(this IEnumerable<IComponent> components, params Type[] componentTypes)
        {
            var existingTypes = new List<Type>();
            foreach (var component in components)
                existingTypes.Add(component.GetType());

            foreach (var type in componentTypes)
                if (!existingTypes.Contains(type))
                    return false;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool HasAny(this IEnumerable<IComponent> components, params Type[] componentTypes)
        {
            var existingTypes = new List<Type>();
            foreach (var component in components)
                existingTypes.Add(component.GetType());

            foreach (var type in componentTypes)
                if (existingTypes.Contains(type))
                    return true;

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public static TComponent Get<TComponent>(this IEnumerable<IComponent> components) where TComponent : IComponent
        {
            foreach (var component in components)
                if (component is TComponent resultComponent)
                    return resultComponent;

            return default;
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool TryGet<TComponent>(this IEnumerable<IComponent> components, out TComponent result) where TComponent : IComponent
        {
            result = components.Get<TComponent>();
            return result != null;
        }
    }
}
