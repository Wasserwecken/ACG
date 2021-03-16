using System.Collections.Generic;
using System.Diagnostics;

namespace Framework.ECS.Components.Relation
{
    [DebuggerDisplay("Parent: {Parent?.Name}")]
    public class ChildComponent : IComponent
    {
        public Entity Parent { get; set; }
    }
}
