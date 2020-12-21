using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public class TransformIndicator
    {
        public TransformComponent Transform { get; set; }
        public TransformIndicatorObject IndicatorObject { get; set; }
        public Material Material { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public void Draw()
        {
            IndicatorObject.Draw();
        }
    }
}
