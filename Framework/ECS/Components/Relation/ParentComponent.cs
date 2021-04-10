﻿using DefaultEcs;
using System.Collections.Generic;
using System.Diagnostics;

namespace Framework.ECS.Components.Relation
{
    [DebuggerDisplay("Children: {Children.Count}")]
    public struct ParentComponent
    {
        public List<Entity> Children { get; set; }
    }
}
