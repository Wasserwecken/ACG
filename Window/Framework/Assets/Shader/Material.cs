using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Framework
{
    public class Material
    {
        public MaterialData Data { get; set; }

        private readonly Dictionary<int, float> _uniformFloats;
        private readonly Dictionary<int, Matrix4> _uniformMat4s;
        private readonly Dictionary<int, Texture> _uniformTextures;

        /// <summary>
        /// 
        /// </summary>
        public Material()
        {
            _uniformFloats = new Dictionary<int, float>();
            _uniformMat4s = new Dictionary<int, Matrix4>();
            _uniformTextures = new Dictionary<int, Texture>();
        }

        public void SetWorldSpace(Matrix4 transform) => SetUniform(Definitions.Shader.Uniforms.Space.WORLD, transform);
        public void SetProjectionSpace(Matrix4 transform) => SetUniform(Definitions.Shader.Uniforms.Space.PROJECTION, transform);

        public void SetUniform(string name, float value) => TrySetUniform(name, value, _uniformFloats);
        public void SetUniform(string name, Matrix4 value) => TrySetUniform(name, value, _uniformMat4s);
        public void SetUniform(string name, Texture texture) => TrySetUniform(name, texture, _uniformTextures);

        /// <summary>
        /// 
        /// 
        /// </summary>
        public void Use(ref RenderData renderData)
        {
            GL.FrontFace(Data.FaceDirection);

            if (Data.IsDepthTesting)
                GL.Enable(EnableCap.DepthTest);

            if (Data.IsCulling)
            {
                GL.Enable(EnableCap.CullFace);
                GL.CullFace(Data.CullingMode);
            }

            if (Data.IsTransparent)
            {
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(Data.SourceBlend, Data.DestinationBlend);
            }
            else
                GL.Disable(EnableCap.Blend);


            foreach (var uniform in _uniformTextures)
                UpdateTextureUniform(uniform.Key, uniform.Value);
            
            foreach (var uniform in _uniformFloats)
                GL.Uniform1(Data.Shader.Uniforms[uniform.Key].Layout, uniform.Value);

            foreach (var uniform in _uniformMat4s)
            {
                var matrix = uniform.Value;
                GL.UniformMatrix4(Data.Shader.Uniforms[uniform.Key].Layout, false, ref matrix);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void TrySetUniform<TValue>(string name, TValue value, IDictionary<int, TValue> uniforms)
        {
            if (Data.Shader.GetUniform(name, out var uniform))
            {
                if (uniforms.ContainsKey(uniform.Layout))
                    uniforms[uniform.Layout] = value;
                else
                    uniforms.Add(uniform.Layout, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateTextureUniform(int layout, Texture texture)
        {
            GL.Uniform1(layout, layout);
            GL.ActiveTexture(TextureUnit.Texture0 + layout);
            GL.BindTexture(TextureTarget.Texture2D, texture.Handle);
        }
    }
}
