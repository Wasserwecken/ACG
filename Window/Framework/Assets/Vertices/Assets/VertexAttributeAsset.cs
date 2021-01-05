using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
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
        public VertexAttributeAsset(
            string name,
            int layout,
            int elementSize,
            bool isNormalized,
            VertexAttribPointerType pointerType)
        {
            Name = name;
            Layout = layout;
            PointerType = pointerType;
            IsNormalized = isNormalized;
            Dimension = elementSize / 4; // because bytes
            ElementSize = elementSize;
            Data = new byte[0];
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetData<TType>(TType[] data) where TType : struct
        {
            var typeSize = Marshal.SizeOf<TType>();
            Data = new byte[data.Length * typeSize];
            System.Buffer.BlockCopy(data, 0, Data, 0, Data.Length);
        }
    }
}
