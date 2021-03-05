using System.Collections.Generic;
using System.Diagnostics;

namespace Framework.Assets.Verticies
{
    [DebuggerDisplay("Name: {Name}, Primitives: {Primitives.Count}")]
    public class MeshAsset
    {
        public string Name { get; set; }
        public List<VertexPrimitiveAsset> Primitives { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MeshAsset(string name)
        {
            Name = name;
            Primitives = new List<VertexPrimitiveAsset>();
        }
    }
}
