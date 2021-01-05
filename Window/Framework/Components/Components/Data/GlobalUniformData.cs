using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public struct GlobalUniformData
    {
        public ShaderBlock<ShaderTime> TimeBlock;
        public ShaderBlock<ShaderSpace> SpaceBlock;
    }
}
