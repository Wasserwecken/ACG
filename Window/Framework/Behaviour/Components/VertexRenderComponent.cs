using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    /// <summary>
    /// 
    /// </summary>
    public struct VertexRenderComponent : IEntityComponent
    {
        public Dictionary<VertexPrimitiveAsset, MaterialAsset> PrimitivesWithMaterials;
    }
}
