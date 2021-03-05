using System;
using System.Diagnostics;
using System.IO;
using OpenTK.Graphics.OpenGL;

namespace Framework.Assets.Shader
{
    [DebuggerDisplay("Type: {Type}, Handle: {Handle}, FilePath: {FilePath}")]
    public class ShaderSourceAsset
    {
        public int Handle;
        public ShaderType Type;
        public string FilePath;
        public string Content;

        /// <summary>
        /// 
        /// </summary>
        public ShaderSourceAsset(ShaderType type, string filePath)
        {
            FilePath = filePath;
            Type = type;

            Load();
            Compile();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Load()
        {
            Content = string.Empty;
            var file = new FileInfo(FilePath);
            if (file.Exists)
            {
                using var reader = new StreamReader(FilePath);
                Content = reader.ReadToEnd();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Compile()
        {
            Handle = GL.CreateShader(Type);
            GL.ShaderSource(Handle, Content);
            GL.CompileShader(Handle);
            GL.GetShaderInfoLog(Handle, out var log);

            if (log != string.Empty)
                Console.WriteLine($"{Type}: {log}");
        }
    }
}
