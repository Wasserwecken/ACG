using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public struct DirectionalLightData
    {
        public static DirectionalLightData Default => new DirectionalLightData()
        {
            Color = new Vector3(0.6f),
            Direction = new Vector3(1f, -1f, 1f)
        };

        public Vector3 Color { get; set; }
        public Vector3 Direction { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DirectionalLightData(Vector3 color, Vector3 direction)
        {
            Color = color;
            Direction = direction;
        }
    }
}
