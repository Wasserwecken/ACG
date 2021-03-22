using System.Diagnostics;
using Framework.Extensions;
using OpenTK.Graphics.OpenGL;

namespace Framework.Assets.Verticies.Attributes
{
    [DebuggerDisplay("Layout: {Layout}, PointerType: {PointerType}, Name: {Name}, ElementCount: {ElementCount}")]
    public abstract class VertexAttributeAsset<TDataType> : IVertexAttribute where TDataType : struct
    {
        public virtual int Layout { get; }
        public virtual string Name { get; }
        public virtual VertexAttribPointerType PointerType { get; }
        public virtual int Dimension { get; }
        public virtual int ElementSize { get; }
        public virtual bool IsNormalized { get; }
        public virtual int ElementCount => DataTyped.Length;
        public virtual byte[] DataBytes { get; protected set; }
        public virtual TDataType[] DataTyped { get; }

        /// <summary>
        /// 
        /// </summary>
        public VertexAttributeAsset(string name, int layout, int elementSize, bool isNormalized, VertexAttribPointerType pointerType)
            : this(name, layout, elementSize, isNormalized, pointerType, new TDataType[0]) { }

        /// <summary>
        /// 
        /// </summary>
        public VertexAttributeAsset(string name, int layout, int elementSize, bool isNormalized, VertexAttribPointerType pointerType, TDataType[] data)
        {
            Name = name;
            Layout = layout;
            PointerType = pointerType;
            IsNormalized = isNormalized;
            Dimension = elementSize / 4; // because bytes
            ElementSize = elementSize;
            DataTyped = data;
        }
    }
}
