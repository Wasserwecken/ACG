using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public int ElementCount { get; set; }
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
        }
    }
}
