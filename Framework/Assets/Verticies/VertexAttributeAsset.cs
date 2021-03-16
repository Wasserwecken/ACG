using System.Diagnostics;
using OpenTK.Graphics.OpenGL;

namespace Framework.Assets.Verticies
{
    [DebuggerDisplay("Layout: {Layout}, PointerType: {PointerType}, Name: {Name}, ElementCount: {ElementCount}")]
    public class VertexAttributeAsset
    {
        public int Layout { get; }
        public VertexAttribPointerType PointerType { get; }
        public string Name { get; }
        public int Dimension { get; }
        public int ElementSize { get; }
        public bool IsNormalized { get; }

        public int ElementCount => Data.Length / ElementSize;
        public byte[] Data { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public VertexAttributeAsset(string name, int layout, int elementSize, bool isNormalized, VertexAttribPointerType pointerType)
            : this(name, layout, elementSize, isNormalized, pointerType, new byte[0]) { }

        /// <summary>
        /// 
        /// </summary>
        public VertexAttributeAsset(string name, int layout, int elementSize, bool isNormalized, VertexAttribPointerType pointerType, byte[] data)
        {
            Name = name;
            Layout = layout;
            PointerType = pointerType;
            IsNormalized = isNormalized;
            Dimension = elementSize / 4; // because bytes
            ElementSize = elementSize;
            Data = data;
        }

        /// <summary>
        /// 
        /// </summary>
        public static VertexAttributeAsset CreatePosition<TType>(TType[] data) where TType : struct
        {
            return new VertexAttributeAsset(
                Definitions.Buffer.VertexAttribute.Position.Name,
                Definitions.Buffer.VertexAttribute.Position.Layout,
                Definitions.Buffer.VertexAttribute.Position.Size,
                Definitions.Buffer.VertexAttribute.Position.Normalize,
                Definitions.Buffer.VertexAttribute.Position.PointerType,
                data.ToBytes()
            );
        }

        /// <summary>
        /// 
        /// </summary>
        public static VertexAttributeAsset CreateNormal<TType>(TType[] data) where TType : struct
        {
            return new VertexAttributeAsset(
                Definitions.Buffer.VertexAttribute.Normal.Name,
                Definitions.Buffer.VertexAttribute.Normal.Layout,
                Definitions.Buffer.VertexAttribute.Normal.Size,
                Definitions.Buffer.VertexAttribute.Normal.Normalize,
                Definitions.Buffer.VertexAttribute.Normal.PointerType,
                data.ToBytes()
            );
        }
        /// <summary>
        /// 
        /// </summary>
        public static VertexAttributeAsset CreateTangent<TType>(TType[] data) where TType : struct
        {
            return new VertexAttributeAsset(
                Definitions.Buffer.VertexAttribute.Tangent.Name,
                Definitions.Buffer.VertexAttribute.Tangent.Layout,
                Definitions.Buffer.VertexAttribute.Tangent.Size,
                Definitions.Buffer.VertexAttribute.Tangent.Normalize,
                Definitions.Buffer.VertexAttribute.Tangent.PointerType,
                data.ToBytes()
            );
        }
        /// <summary>
        /// 
        /// </summary>
        public static VertexAttributeAsset CreateUV<TType>(TType[] data) where TType : struct
        {
            return new VertexAttributeAsset(
                Definitions.Buffer.VertexAttribute.UV.Name,
                Definitions.Buffer.VertexAttribute.UV.Layout,
                Definitions.Buffer.VertexAttribute.UV.Size,
                Definitions.Buffer.VertexAttribute.UV.Normalize,
                Definitions.Buffer.VertexAttribute.UV.PointerType,
                data.ToBytes()
            );
        }
        /// <summary>
        /// 
        /// </summary>
        public static VertexAttributeAsset CreateColor<TType>(TType[] data) where TType : struct
        {
            return new VertexAttributeAsset(
                Definitions.Buffer.VertexAttribute.Color.Name,
                Definitions.Buffer.VertexAttribute.Color.Layout,
                Definitions.Buffer.VertexAttribute.Color.Size,
                Definitions.Buffer.VertexAttribute.Color.Normalize,
                Definitions.Buffer.VertexAttribute.Color.PointerType,
                data.ToBytes()
            );
        }
    }
}
