using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL;

namespace Framework.Assets.Verticies.Attributes
{
    [DebuggerDisplay("Layout: {Layout}, PointerType: {PointerType}, Name: {Name}, ElementCount: {ElementCount}")]
    public class VertexAttribute<TDataType> : IVertexAttribute where TDataType : struct
    {
        public int Layout { get; }
        public string Name { get; }
        public VertexAttribPointerType PointerType { get; }
        public int Dimension { get; }
        public int ElementSize { get; }
        public bool Normalize { get; }
        public int ElementCount => DataTyped.Length;
        public TDataType[] DataTyped { get; set; }
        public byte[] DataBytes { get; protected set; }
        public Func<TDataType, byte[]> ByteConverter { get; }

        /// <summary>
        /// 
        /// </summary>
        public VertexAttribute(
            string name,
            int layout,
            bool normalize,
            int elementSize,
            VertexAttribPointerType pointerType,
            Func<TDataType, byte[]> byteConverter)
        {
            Name = name;
            Layout = layout;
            PointerType = pointerType;
            Normalize = normalize;
            Dimension = elementSize / 4; // because bytes
            ElementSize = elementSize;
            ByteConverter = byteConverter;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void UpdateByteData()
        {
            var byteList = new List<byte>();
            for (int i = 0; i < DataTyped.Length; i++)
                byteList.AddRange(ByteConverter(DataTyped[i]));

            DataBytes = byteList.ToArray();
        }
    }
}
