using System;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public static class VertexAttributeManager
    {
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
