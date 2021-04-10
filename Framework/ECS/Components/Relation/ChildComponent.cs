using DefaultEcs;
using System.Diagnostics;

namespace Framework.ECS.Components.Relation
{
    [DebuggerDisplay("Parent: {Parent?.Name}")]
    public struct ChildComponent
    {
        public Entity Parent { get; set; }
    }
}
