using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public struct MaterialData
    {
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
        public static MaterialData Initial() => Initial(null);

        /// <summary>
        /// 
        /// </summary>
        public static MaterialData Initial(ShaderProgram shader)
        {
            var material = new MaterialData();
            material.Reset();
            material.Shader = shader;
            return material;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            IsTransparent = false;
            IsCulling = true;
            IsDepthTesting = true;
            CullingMode = CullFaceMode.Back;
            FaceDirection = FrontFaceDirection.Cw;
            SourceBlend = BlendingFactor.SrcAlpha;
            DestinationBlend = BlendingFactor.OneMinusSrcAlpha;
        }
    }
}
