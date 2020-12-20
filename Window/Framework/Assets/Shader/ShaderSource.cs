using System;
using System.IO;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public class ShaderSource : IDisposable
    {
        public ShaderType Type { get; private set; }
        public string Path { get; private set; }
        public int Handle { get; private set; }
        public string Content { get; private set; }
        public string Log { get; private set; }
        public bool IsValid => Log == string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public ShaderSource(ShaderType shaderType, string filePath)
        {
            Type = shaderType;
            Path = filePath;
            Handle = GL.CreateShader(Type);
            Content = ReadFile(Path);

            GL.ShaderSource(Handle, Content);
            GL.CompileShader(Handle);
            GL.GetShaderInfoLog(Handle, out var log);

            if (log != string.Empty)
                Log = $"{Type}:\n {log}";
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
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {

        }
    }
}
