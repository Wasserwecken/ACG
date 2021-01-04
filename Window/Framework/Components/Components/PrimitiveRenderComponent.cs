using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    /// <summary>
    /// 
    /// </summary>
    public struct PrimitiveRenderComponent : IEntityComponent
    {
        public VertexPrimitiveAsset Primitive;
        public MaterialAsset Material;
    }
}
