using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public struct GlobalUniformData
    {
        ShaderStorageBuffer<TimeStorageBlock> TimeBlock { get; set; }
    }
}
