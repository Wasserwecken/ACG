using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public struct GlobalUniformData
    {
        public UniformBlock<TimeData> TimeBlock;
        public UniformBlock<RenderSpaceData> SpaceBlock;
    }
}
