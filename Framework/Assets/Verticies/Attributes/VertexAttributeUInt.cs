using System;

namespace Framework.Assets.Verticies.Attributes
{
    public class VertexAttributeUInt : VertexAttribute<uint>
    {
        /// <summary>
        /// 
        /// </summary>
        public VertexAttributeUInt(string name, int layout, bool normalize)
            : base(name, layout, normalize,
                  Definitions.Shader.Attribute.UInt.Size, Definitions.Shader.Attribute.UInt.PointerType,
                  BitConverter.GetBytes)
        { }
    }
}
