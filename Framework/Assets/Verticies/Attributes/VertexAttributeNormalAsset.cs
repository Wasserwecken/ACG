using Framework.Extensions;
using OpenTK.Mathematics;

namespace Framework.Assets.Verticies.Attributes
{
    public class VertexAttributeNormalAsset : VertexAttributeAsset<Vector3>
    {
        /// <summary>
        /// 
        /// </summary>
        public VertexAttributeNormalAsset()
            : base(Definitions.Shader.Attribute.Normal.Name,
                  Definitions.Shader.Attribute.Normal.Layout,
                  Definitions.Shader.Attribute.Normal.Size,
                  Definitions.Shader.Attribute.Normal.Normalize,
                  Definitions.Shader.Attribute.Normal.PointerType)
        { }

        /// <summary>
        /// 
        /// </summary>
        public VertexAttributeNormalAsset(Vector3[] data)
            : base(Definitions.Shader.Attribute.Normal.Name,
                  Definitions.Shader.Attribute.Normal.Layout,
                  Definitions.Shader.Attribute.Normal.Size,
                  Definitions.Shader.Attribute.Normal.Normalize,
                  Definitions.Shader.Attribute.Normal.PointerType,
                  data)
        {
            DataBytes = DataTyped.ToBytes();
        }
    }
}
