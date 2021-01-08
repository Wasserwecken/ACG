using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public class MaterialTypedAsset<TSettings> : MaterialAsset where TSettings : struct
    {
        public ShaderBlock<TSettings> SettingsBlock;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public MaterialTypedAsset(string name, ShaderProgramAsset shader) : base(name, shader) { }
    }
}
