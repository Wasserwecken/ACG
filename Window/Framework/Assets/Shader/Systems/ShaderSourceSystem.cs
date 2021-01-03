using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public static class ShaderSourceSystem
    {
        private static readonly Dictionary<string, ShaderSourceAsset> _sources;

        /// <summary>
        /// 
        /// </summary>
        static ShaderSourceSystem()
        {
            _sources = new Dictionary<string, ShaderSourceAsset>();
        }

        /// <summary>
        /// 
        /// </summary>
        public static ShaderSourceAsset Create(ShaderType type, string filePath)
        {
            if (_sources.TryGetValue(filePath, out var source))
                return source;
            else
            {
                var content = ReadFile(filePath);
                var shaderSource = new ShaderSourceAsset(type, GL.CreateShader(type), filePath, content);
                CompileShaderSource(shaderSource);

                _sources.Add(filePath, shaderSource);
                return shaderSource;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static string ReadFile(string path)
        {
            var file = new FileInfo(path);
            if (file.Exists)
            {
                using var reader = new StreamReader(path);
                return reader.ReadToEnd();
            }
            else
                return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void CompileShaderSource(ShaderSourceAsset shaderSource)
        {
            GL.ShaderSource(shaderSource.Handle, shaderSource.Content);
            GL.CompileShader(shaderSource.Handle);
            GL.GetShaderInfoLog(shaderSource.Handle, out var log);

            if (log != string.Empty)
                Console.WriteLine($"{shaderSource.Type}: {log}");
        }
    }
}
