using System;

namespace Framework.Assets.Verticies.Attributes
{
    public class VertexAttributeFloat : VertexAttribute<float>
    {
        /// <summary>
        /// 
        /// </summary>
        public VertexAttributeFloat(string name, int layout, bool normalize)
            : base(name, layout, normalize,
                  Definitions.Shader.Attribute.Float.Size,
                  Definitions.Shader.Attribute.Float.PointerType,
                  BitConverter.GetBytes)
        { }
    }
}
