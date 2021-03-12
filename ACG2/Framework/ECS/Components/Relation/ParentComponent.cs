using System.Collections.Generic;
using System.Diagnostics;

namespace Framework.ECS.Components.Relation
{
    [DebuggerDisplay("Children: {Children.Count}")]
    public class ParentComponent : IComponent
    {
        public List<Entity> Children { get; set; }
    }
}
