using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public struct ShaderMaterialPBRSettings
    {
        public Vector4 BaseColor;
        public Vector4 Emissive;
        public float Metallic;
        public float Roughness;
        public float Normal;
        public float Occlusion;
    }
}
