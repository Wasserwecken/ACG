using OpenTK.Mathematics;
using Framework.Extensions;

namespace Framework.Assets.Verticies.Attributes
{
    public class VertexAttributeVector3 : VertexAttribute<Vector3>
    {
        /// <summary>
        /// 
        /// </summary>
        public VertexAttributeVector3(string name, int layout, bool normalize)
            : base(name, layout, normalize,
                  Definitions.Shader.Attribute.Vector3.Size,
                  Definitions.Shader.Attribute.Vector3.PointerType,
                  ConversionExtensions.ToBytes)
        { }
    }
}
