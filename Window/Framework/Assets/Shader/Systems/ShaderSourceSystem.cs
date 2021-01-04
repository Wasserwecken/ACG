using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public static class ShaderSourceSystem
    {
        /// <summary>
        /// 
        /// </summary>
        public static void Load(ShaderSourceAsset shaderSource)
        {
            var file = new FileInfo(shaderSource.FilePath);
            if (file.Exists)
            {
                using var reader = new StreamReader(shaderSource.FilePath);
                shaderSource.Content = reader.ReadToEnd();
            }
            else
                shaderSource.Content = string.Empty;
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

        /// <summary>
        /// 
        /// </summary>
        public static void LoadAndCompile(ShaderSourceAsset shaderSource)
        {
            Load(shaderSource);
            Compile(shaderSource);
        }
    }
}
