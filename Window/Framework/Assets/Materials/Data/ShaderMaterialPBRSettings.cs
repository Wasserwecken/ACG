using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public struct ShaderMaterialPBRSettings
    {
        public Vector4 BaseColor;
        public float Metallic;
        public float Roughness;
        public float Occlusion;
        public float Emissive;
        public float Normal;
    }
}
