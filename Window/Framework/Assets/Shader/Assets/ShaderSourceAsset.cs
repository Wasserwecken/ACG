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
        public string FilePath { get; }
        public int Handle { get; set; }
        public string Content { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ShaderSourceAsset(
            ShaderType type,
            string filePath)
        {
            Type = type;
            FilePath = filePath;
        }
    }
}
