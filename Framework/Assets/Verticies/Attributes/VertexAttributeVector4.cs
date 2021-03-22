using OpenTK.Mathematics;
using Framework.Extensions;

namespace Framework.Assets.Verticies.Attributes
{
    public class VertexAttributeVector4 : VertexAttribute<Vector4>
    {
        /// <summary>
        /// 
        /// </summary>
        public VertexAttributeVector4(string name, int layout, bool normalize)
            : base(name, layout, Definitions.Shader.Attribute.Vector4.Size, normalize, Definitions.Shader.Attribute.Vector4.PointerType, ConversionExtensions.ToBytes)
        { }
    }
}
