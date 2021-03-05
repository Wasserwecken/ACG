using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public static class ShaderSourceManager
    {
        /// <summary>
        /// 
        /// </summary>
        public static ShaderSourceAsset Create(ShaderType type, string filePath)
        {
            var content = string.Empty;
            var file = new FileInfo(filePath);
            if (file.Exists)
            {
                using var reader = new StreamReader(filePath);
                content = reader.ReadToEnd();
            }

            var source = new ShaderSourceAsset()
            {
                FilePath = filePath,
                Type = type,
                Content = content
            };

            Compile(source);
            return source;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Reload(ShaderSourceAsset source)
        {
            source.Handle = -1;
            source.Content = string.Empty;
            
            var file = new FileInfo(source.FilePath);
            if (file.Exists)
            {
                using var reader = new StreamReader(source.FilePath);
                source.Content = reader.ReadToEnd();
            }

            Compile(source);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Compile(ShaderSourceAsset shaderSource)
        {
            shaderSource.Handle = GL.CreateShader(shaderSource.Type);
            GL.ShaderSource(shaderSource.Handle, shaderSource.Content);
            GL.CompileShader(shaderSource.Handle);
            GL.GetShaderInfoLog(shaderSource.Handle, out var log);

            if (log != string.Empty)
                Console.WriteLine($"{shaderSource.Type}: {log}");
        }
    }
}
