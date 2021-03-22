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
        public static bool Has<TComponent>(this IEnumerable<IComponent> components, out TComponent result) where TComponent : IComponent
        {
            foreach (var component in components)
            {
                if (component is TComponent)
                {
                    result = (TComponent)component;
                    return true;
                }
            }
            
            result = default;
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
    }
}
