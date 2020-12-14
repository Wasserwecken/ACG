using System.Runtime.InteropServices;

namespace Framework
{
    public class VertexAttribute<TType> where TType : struct
    {
        public int Layout { get; private set; }
        public string Name { get; private set; }
        public bool IsNormalized { get; private set; }
        public int TypeSize { get; private set; }
        public TType[] Data { get; private set; }
        public int PairLength { get; private set; }
        public int PairCount { get; private set; }
        public int PairSize { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public VertexAttribute(int layout, string name, int pairLength, bool isNormalized, TType[] data)
        {
            Layout = layout;
            Name = name;
            IsNormalized = isNormalized;
            TypeSize = Marshal.SizeOf(default(TType));
            Data = data;
            PairLength = pairLength;
            PairCount = Data.Length / pairLength;
            PairSize = pairLength * TypeSize;
        }
    }
}
