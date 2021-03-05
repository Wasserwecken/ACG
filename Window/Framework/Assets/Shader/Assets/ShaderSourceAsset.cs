using System;
using System.Diagnostics;
using System.IO;
using OpenTK.Graphics.OpenGL;

namespace Framework
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
        public ShaderSourceAsset()
        {
            Handle = -1;
        }
    }
}
