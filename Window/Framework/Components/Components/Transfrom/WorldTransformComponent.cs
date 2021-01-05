using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Framework
{
    public struct WorldTransformComponent : IEntityComponent
    {
        public Matrix4 Space;
    }
}
