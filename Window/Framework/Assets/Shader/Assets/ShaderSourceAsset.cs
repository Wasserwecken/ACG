using System;
using System.Diagnostics;
using System.IO;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    [DebuggerDisplay("Type: {Type}, Handle: {Handle}, FilePath: {FilePath}")]
    public class ShaderSourceAsset
    {
        public ShaderType Type { get; }
        public int Handle { get; }
        public string FilePath { get; }
        public string Content { get; }

        /// <summary>
        /// 
        /// </summary>
        public ShaderSourceAsset(
            ShaderType type,
            int handle,
            string filePath,
            string content)
        {
            Type = type;
            Handle = handle;
            FilePath = filePath;
            Content = content;
        }
    }
}
