using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public struct LightData
    {
        private const int VEC3_SIZE = sizeof(float) * 3;

        public Vector3 AmbientColor { get; private set; }

        public int DirectionalCount { get; private set; }
        public float[] DirectionalColors { get; private set; }
        public float[] DirectionalDirections { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="light"></param>
        public void SetAmbient(AmbientLightData light)
        {
            AmbientColor = light.Color;
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetDirectional(params DirectionalLightData[] lights)
        {
            DirectionalCount = lights.Length;
            DirectionalColors = new float[lights.Length * 3];
            DirectionalDirections = new float[lights.Length * 3];
            
            for (int i = 0; i < lights.Length; i++)
            {
                var d = i * 3;

                DirectionalColors[d + 0] = lights[i].Color.X;
                DirectionalColors[d + 1] = lights[i].Color.Y;
                DirectionalColors[d + 2] = lights[i].Color.Z;
                
                DirectionalDirections[d + 0] = lights[i].Direction.X;
                DirectionalDirections[d + 1] = lights[i].Direction.Y;
                DirectionalDirections[d + 2] = lights[i].Direction.Z;
            }

        }
    }
}
