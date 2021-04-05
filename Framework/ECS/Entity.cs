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
    }
}
