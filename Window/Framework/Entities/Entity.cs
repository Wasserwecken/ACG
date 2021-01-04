using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Framework
{
    [DebuggerDisplay("Name: {Name}, Children: {Children.Count}, LocalPosition: {LocalTransform.Position}")]
    public class Entity
    {
        public string Name { get; set; }
        public List<Entity> Children { get; }
        public TransformComponent WorldTransform { get; set; }
        public TransformComponent LocalTransform { get; set; }
        public readonly List<IEntityComponent> Components;

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
            Children = new List<Entity>();

            if (initialComponents != null)
                Components = new List<IEntityComponent>(initialComponents);
            else
                Components = new List<IEntityComponent>();
        }

        /// <summary>
        /// 
        /// </summary>
        public List<IEntityComponent> GetComponents<TComponentType>() where TComponentType : IEntityComponent
        {
            var results = new List<IEntityComponent>();

            foreach (var component in Components)
                if (component is TComponentType)
                    results.Add(component);

            return results;
        }
    }
}
