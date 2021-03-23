using OpenTK.Mathematics;
using Framework.Extensions;

namespace Framework.Assets.Verticies.Attributes
{
    public class VertexAttributeVector2 : VertexAttribute<Vector2>
    {
        /// <summary>
        /// 
        /// </summary>
        public VertexAttributeVector2(string name, int layout, bool normalize)
            : base(name, layout, normalize,
                  Definitions.Shader.Attribute.Vector2.Size,
                  Definitions.Shader.Attribute.Vector2.PointerType,
                  ConversionExtensions.ToBytes)
        { }
    }
}
