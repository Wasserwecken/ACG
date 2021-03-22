using Framework.Extensions;
using OpenTK.Mathematics;

namespace Framework.Assets.Verticies.Attributes
{
    public class VertexAttributeTangentAsset : VertexAttributeAsset<Vector4>
    {
        /// <summary>
        /// 
        /// </summary>
        public VertexAttributeTangentAsset()
            : base(Definitions.Shader.Attribute.Tangent.Name,
                  Definitions.Shader.Attribute.Tangent.Layout,
                  Definitions.Shader.Attribute.Tangent.Size,
                  Definitions.Shader.Attribute.Tangent.Normalize,
                  Definitions.Shader.Attribute.Tangent.PointerType)
        { }

        /// <summary>
        /// 
        /// </summary>
        public VertexAttributeTangentAsset(Vector4[] data)
            : base(Definitions.Shader.Attribute.Tangent.Name,
                  Definitions.Shader.Attribute.Tangent.Layout,
                  Definitions.Shader.Attribute.Tangent.Size,
                  Definitions.Shader.Attribute.Tangent.Normalize,
                  Definitions.Shader.Attribute.Tangent.PointerType,
                  data)
        {
            DataBytes = DataTyped.ToBytes();
        }
    }
}
