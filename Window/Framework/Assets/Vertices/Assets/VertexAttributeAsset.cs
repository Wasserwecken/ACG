using System.Diagnostics;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;

namespace Framework
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
    }
}
