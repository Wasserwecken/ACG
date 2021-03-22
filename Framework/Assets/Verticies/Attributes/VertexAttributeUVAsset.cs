using Framework.Extensions;
using OpenTK.Mathematics;

namespace Framework.Assets.Verticies.Attributes
{
    public class VertexAttributeUVAsset : VertexAttributeAsset<Vector2>
    {
        /// <summary>
        /// 
        /// </summary>
        public VertexAttributeUVAsset()
            : base(Definitions.Shader.Attribute.UV.Name,
                  Definitions.Shader.Attribute.UV.Layout,
                  Definitions.Shader.Attribute.UV.Size,
                  Definitions.Shader.Attribute.UV.Normalize,
                  Definitions.Shader.Attribute.UV.PointerType)
        { }

        /// <summary>
        /// 
        /// </summary>
        public VertexAttributeUVAsset(Vector2[] data)
            : base(Definitions.Shader.Attribute.UV.Name,
                  Definitions.Shader.Attribute.UV.Layout,
                  Definitions.Shader.Attribute.UV.Size,
                  Definitions.Shader.Attribute.UV.Normalize,
                  Definitions.Shader.Attribute.UV.PointerType,
                  data)
        {
            DataBytes = DataTyped.ToBytes();
        }
    }
}
