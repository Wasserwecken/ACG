using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public struct GlobalUniformData
    {
        public ShaderUniformBlock<TimeData> TimeBlock;
        public ShaderUniformBlock<SpaceData> SpaceBlock;
    }
}
