using Framework.Extensions;
using OpenTK.Mathematics;

namespace Framework.Assets.Verticies.Attributes
{
    public class VertexAttributeColorAsset : VertexAttributeAsset<Vector4>
    {
        /// <summary>
        /// 
        /// </summary>
        public VertexAttributeColorAsset()
            : base(Definitions.Shader.Attribute.Color.Name,
                  Definitions.Shader.Attribute.Color.Layout,
                  Definitions.Shader.Attribute.Color.Size,
                  Definitions.Shader.Attribute.Color.Normalize,
                  Definitions.Shader.Attribute.Color.PointerType)
        { }

        /// <summary>
        /// 
        /// </summary>
        public VertexAttributeColorAsset(Vector4[] data)
            : base(Definitions.Shader.Attribute.Color.Name,
                  Definitions.Shader.Attribute.Color.Layout,
                  Definitions.Shader.Attribute.Color.Size,
                  Definitions.Shader.Attribute.Color.Normalize,
                  Definitions.Shader.Attribute.Color.PointerType,
                  data)
        {
            DataBytes = DataTyped.ToBytes();
        }
    }
}
