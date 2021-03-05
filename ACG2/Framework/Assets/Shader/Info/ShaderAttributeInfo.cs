using OpenTK.Graphics.OpenGL;
using System.Diagnostics;

namespace Framework.Assets.Shader.Info
{
    [DebuggerDisplay("Layout: {Layout}, Type: {Type}, Name: {Name}")]
    public struct ShaderAttributeInfo
    {
        public int Layout { get; private set; }
        public ActiveAttribType Type { get; private set; }
        public string Name { get; private set; }
        public int Size { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ShaderAttributeInfo(int layout, ActiveAttribType type, string name, int size)
        {
            Layout = layout;
            Type = type;
            Name = name;
            Size = size;
        }
    }
}
