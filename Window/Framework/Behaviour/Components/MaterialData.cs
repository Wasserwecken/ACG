using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public struct MaterialData
    {
        public static MaterialData Default => new MaterialData()
        {
            IsTransparent = false,
            IsCulling = true,
            IsDepthTesting = true,
            CullingMode = CullFaceMode.Back,
            FaceDirection = FrontFaceDirection.Ccw,
            SourceBlend = BlendingFactor.SrcAlpha,
            DestinationBlend = BlendingFactor.OneMinusSrcAlpha
        };

        public ShaderProgram Shader { get; set; }
        public bool IsTransparent { get; set; }
        public bool IsCulling { get; set; }
        public bool IsDepthTesting { get; set; }
        public CullFaceMode CullingMode { get; set; }
        public FrontFaceDirection FaceDirection { get; set; }
        public BlendingFactor SourceBlend { get; set; }
        public BlendingFactor DestinationBlend { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MaterialData(ShaderProgram shader)
        {
            this = Default;
            Shader = shader;
        }
    }
}
