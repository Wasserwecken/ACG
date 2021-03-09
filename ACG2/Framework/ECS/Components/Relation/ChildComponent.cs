using System.Collections.Generic;
using System.Diagnostics;

namespace Framework.ECS.Components.Relation
{
    [DebuggerDisplay("Children: {Children.Count}")]
    public class ChildComponent : IComponent
    {
        public List<Entity> Children { get; set; }
    }
}
