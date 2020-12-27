using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public struct AmbientLightData
    {
        public static AmbientLightData Default => new AmbientLightData()
        {
            Color = new Vector3(0.03f)
        };

        public Vector3 Color { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public AmbientLightData(Vector3 color)
        {
            Color = color;
        }
    }
}
