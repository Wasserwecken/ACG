using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public struct ShaderUniformBlockInfo
    {
        public int Layout { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ShaderUniformBlockInfo(int layout, string name)
        {
            Layout = layout;
            Name = name;
        }
    }
}
