using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Framework
{
    public class MaterialAsset
    {
        public ShaderProgramAsset Shader { get; }
        public bool IsTransparent { get; set; }
        public bool IsCulling { get; set; }
        public bool IsDepthTesting { get; set; }
        public CullFaceMode CullingMode { get; set; }
        public FrontFaceDirection FaceDirection { get; set; }
        public BlendingFactor SourceBlend { get; set; }
        public BlendingFactor DestinationBlend { get; set; }

        public Dictionary<int, float> UniformFloats;
        public Dictionary<int, Vector3> UniformVec3s;
        public Dictionary<int, Matrix4> UniformMat4s;
        public Dictionary<int, Texture> UniformTextures;

        /// <summary>
        /// 
        /// </summary>
        public MaterialAsset(ShaderProgramAsset shader)
        {
            Shader = shader;

            IsTransparent = false;
            IsCulling = true;
            IsDepthTesting = true;
            CullingMode = CullFaceMode.Back;
            FaceDirection = FrontFaceDirection.Ccw;
            SourceBlend = BlendingFactor.SrcAlpha;
            DestinationBlend = BlendingFactor.OneMinusSrcAlpha;

            UniformFloats = new Dictionary<int, float>();
            UniformVec3s = new Dictionary<int, Vector3>();
            UniformMat4s = new Dictionary<int, Matrix4>();
            UniformTextures = new Dictionary<int, Texture>();
        }

        public void SetUniform(string name, float value) => TrySetUniform(name, value, UniformFloats);
        public void SetUniform(string name, Vector3 value) => TrySetUniform(name, value, UniformVec3s);
        public void SetUniform(string name, Matrix4 value) => TrySetUniform(name, value, UniformMat4s);
        public void SetUniform(string name, Texture texture) => TrySetUniform(name, texture, UniformTextures);

        /// <summary>
        /// 
        /// </summary>
        private void TrySetUniform<TValue>(string name, TValue value, IDictionary<int, TValue> uniforms)
        {
            if (Shader.IdentifierToLayout.TryGetValue(name, out var layout))
            {
                if (uniforms.ContainsKey(layout))
                    uniforms[layout] = value;
                else
                    uniforms.Add(layout, value);
            }
        }
    }
}
