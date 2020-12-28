using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public struct ShaderStorageBlockInfo
    {
        public int Layout { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ShaderStorageBlockInfo(int layout, string name)
        {
            Layout = layout;
            Name = name;
        }
    }
}
