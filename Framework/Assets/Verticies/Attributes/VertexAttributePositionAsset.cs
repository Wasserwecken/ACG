using Framework.Extensions;
using OpenTK.Mathematics;

namespace Framework.Assets.Verticies.Attributes
{
    public class VertexAttributePositionAsset : VertexAttributeAsset<Vector3>
    {
        /// <summary>
        /// 
        /// </summary>
        public VertexAttributePositionAsset()
            : base(Definitions.Shader.Attribute.Position.Name,
                  Definitions.Shader.Attribute.Position.Layout,
                  Definitions.Shader.Attribute.Position.Size,
                  Definitions.Shader.Attribute.Position.Normalize,
                  Definitions.Shader.Attribute.Position.PointerType)
        { }

        /// <summary>
        /// 
        /// </summary>
        public VertexAttributePositionAsset(Vector3[] data)
            : base(Definitions.Shader.Attribute.Position.Name,
                  Definitions.Shader.Attribute.Position.Layout,
                  Definitions.Shader.Attribute.Position.Size,
                  Definitions.Shader.Attribute.Position.Normalize,
                  Definitions.Shader.Attribute.Position.PointerType,
                  data)
        {
            DataBytes = DataTyped.ToBytes();
        }
    }
}
