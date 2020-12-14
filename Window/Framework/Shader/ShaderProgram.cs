﻿using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public class ShaderProgram : IDisposable
    {
        public int Handle { get; private set; }
        public string Log { get; private set; }
        public bool IsValid => Log == string.Empty;

        public ShaderAttribute[] Attributes { get; private set; }
        public ShaderUniform[] Uniforms { get; private set; }

        private readonly Dictionary<string, int> _nameToAttribute;
        private readonly Dictionary<string, int> _nameToUniform;

        /// <summary>
        /// 
        /// </summary>
        public ShaderProgram(params ShaderSource[] shaders)
        {
            _nameToAttribute = new Dictionary<string, int>();
            _nameToUniform = new Dictionary<string, int>();

            Create(shaders);
            Analyse();

            if (!IsValid)
                Log = $"ShaderProgram:\n {Log}";
        }

        /// <summary>
        /// 
        /// </summary>
        public bool GetAttribute(string name, out ShaderAttribute attribute)
        {
            if (_nameToAttribute.TryGetValue(name, out var layout))
            {
                attribute = Attributes[layout];
                return true;
            }
            else
            {
                attribute = null;
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool GetUniform(string name, out ShaderUniform uniform)
        {
            if (_nameToUniform.TryGetValue(name, out var layout))
            {
                uniform = Uniforms[layout];
                return true;
            }
            else
            {
                uniform = null;
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Create(params ShaderSource[] shaders)
        {
            Handle = GL.CreateProgram();
            foreach (var shader in shaders)
                GL.AttachShader(Handle, shader.Handle);

            GL.LinkProgram(Handle);
            GL.GetProgramInfoLog(Handle, out var programLog);
            Log = programLog;

            foreach (var shader in shaders)
                GL.DetachShader(Handle, shader.Handle);
        }

        /// <summary>
        /// 
        /// </summary>
        private void Analyse()
        {
            // Attributes
            GL.GetProgram(Handle, GetProgramParameterName.ActiveAttributes, out int attributeCount);
            Attributes = new ShaderAttribute[attributeCount];
            
            for (int layout = 0; layout < attributeCount; layout++)
            {
                GL.GetActiveAttrib(Handle, layout, 255, out int length, out int size, out var type, out var name);
                Attributes[layout] = new ShaderAttribute(type, layout, name, length, size);
                _nameToAttribute.Add(name, layout);
            }


            // Uniforms
            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out int uniformCount);
            Uniforms = new ShaderUniform[uniformCount];

            for (int layout = 0; layout < uniformCount; layout++)
            {
                GL.GetActiveUniform(Handle, layout, 255, out int length, out int size, out var type, out var name);
                Uniforms[layout] = new ShaderUniform(type, layout, name, length, size);
                _nameToUniform.Add(name, layout);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            GL.DeleteProgram(Handle);
        }
    }
}
