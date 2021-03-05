using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public class IndicieBufferAsset : BufferBase
    {
        /// <summary>
        /// 
        /// </summary>
        public IndicieBufferAsset(BufferUsageHint usageHint, uint[] indicies)
            : this(usageHint)
        {
            Data = indicies.ToBytes();
        }

        /// <summary>
        /// 
        /// </summary>
        public IndicieBufferAsset(BufferUsageHint usageHint)
            : base(usageHint, BufferTarget.ElementArrayBuffer, "Indicies", sizeof(uint)) { }
    }
}
