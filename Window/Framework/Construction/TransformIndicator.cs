using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public class TransformIndicator
    {
        public TransformData Transform { get; set; }
        public TransformIndicatorObject IndicatorObject { get; set; }
        public Material Material { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public void Draw(ref RenderData renderData)
        {
            IndicatorObject.Draw(ref renderData);
        }
    }
}
