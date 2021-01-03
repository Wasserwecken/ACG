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
        public IndicieBufferAsset(BufferUsageHint usageHint)
            : base("Indicies", sizeof(uint), usageHint) { }
    }
}
