using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL;

namespace Framework.Assets.Verticies.Attributes
{
    [DebuggerDisplay("Layout: {Layout}, PointerType: {PointerType}, Name: {Name}, ElementCount: {ElementCount}")]
    public class VertexAttribute<TDataType> : IVertexAttribute where TDataType : struct
    {
        public virtual int Layout { get; }
        public virtual string Name { get; }
        public virtual VertexAttribPointerType PointerType { get; }
        public virtual int Dimension { get; }
        public virtual int ElementSize { get; }
        public virtual bool Normalize { get; }
        public virtual int ElementCount => DataTyped.Length;
        public virtual TDataType[] DataTyped { get; set; }
        public virtual byte[] DataBytes
        {
            get
            {
                var dataHash = DataTyped.GetHashCode();
                
                if (dataHash != _lastDataHash)
                {
                    _lastDataHash = dataHash;

                    var foo = new List<byte>();
                    for (int i = 0; i < DataTyped.Length; i++)
                        foo.AddRange(_byteConverter(DataTyped[i]));
                    _dataBytes = foo.ToArray();
                }

                return _dataBytes;
            }
        }

        protected Func<TDataType, byte[]> _byteConverter;
        protected int _lastDataHash;
        protected byte[] _dataBytes;

        /// <summary>
        /// 
        /// </summary>
        public VertexAttribute(
            string name,
            int layout,
            int elementSize,
            bool normalize,
            VertexAttribPointerType pointerType,
            Func<TDataType, byte[]> byteConverter)
        {
            Name = name;
            Layout = layout;
            PointerType = pointerType;
            Normalize = normalize;
            Dimension = elementSize / 4; // because bytes
            ElementSize = elementSize;

            _byteConverter = byteConverter;
        }
    }
}
