using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public class Material
    {
        public ShaderProgram Shader { get; set; }

        private readonly Dictionary<int, float> _uniformFloats;
        private readonly Dictionary<int, Texture> _uniformTextures;

        /// <summary>
        /// 
        /// </summary>
        public Material(ShaderProgram shader)
        {
            Shader = shader;

            _uniformFloats = new Dictionary<int, float>();
            _uniformTextures = new Dictionary<int, Texture>();
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetUniformTime(float seconds) => SetUniform(Definitions.Shader.Uniforms.Time.NAME, seconds);

        /// <summary>
        /// 
        /// </summary>
        public void SetUniform(string name, float value) => TrySetUniform(name, value, _uniformFloats);

        /// <summary>
        /// 
        /// </summary>
        public void SetUniform(string name, Texture texture) => TrySetUniform(name, texture, _uniformTextures);

        /// <summary>
        /// 
        /// 
        /// </summary>
        public void Use()
        {
            GL.UseProgram(Shader.Handle);

            foreach (var uniform in _uniformFloats)
                GL.Uniform1(Shader.Uniforms[uniform.Key].Layout, uniform.Value);

            foreach (var uniform in _uniformTextures)
                UpdateTextureUniform(uniform.Key, uniform.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        private void TrySetUniform<TValue>(string name, TValue value, IDictionary<int, TValue> uniforms)
        {
            if (Shader.GetUniform(name, out var uniform))
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
