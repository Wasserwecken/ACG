using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public class VertexAttribute
    {
        public int Layout { get; private set; }
        public bool IsNormalized { get; private set; }
        public int TypeSize { get; private set; }
        public VertexAttribPointerType PointerType { get; private set; }

        public int PairLength { get; private set; }
        public int PairSize { get; private set; }
        public int PairCount { get; private set; }
        
        public byte[] Data { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public static VertexAttribute Create<TType>(int layout, int pairLength, bool isNormalized, VertexAttribPointerType pointerType, TType[] data) where TType : struct
        {
            var typeSize = Marshal.SizeOf(default(TType));
            var attributeData = new byte[typeSize * data.Length];

            System.Buffer.BlockCopy(data, 0, attributeData, 0, attributeData.Length);

            var attribute = new VertexAttribute()
            {
                Layout = layout,
                PointerType = pointerType,
                IsNormalized = isNormalized,
                TypeSize = typeSize,

                PairLength = pairLength,
                PairSize = pairLength * typeSize,
                PairCount = data.Length / pairLength,

                Data = attributeData
            };

            return attribute;
        }
    }
}
