using System.Diagnostics;

namespace Framework.ECS.Components.Relation
{
    [DebuggerDisplay("Parent: {Parent?.Name}")]
    public class ParentComponent : IComponent
    {
        public Entity Parent { get; set; }
    }
}
