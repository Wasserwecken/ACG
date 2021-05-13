using System.Diagnostics;
using OpenTK.Graphics.OpenGL;

namespace Framework.Assets.Shader.Info
{
    [DebuggerDisplay("Layout: {Layout}, Target: {Target}, Name: {Name}")]
    public struct ShaderBlockInfo
    {
        public int Layout { get; set; }
        public BufferTarget Target { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ShaderBlockInfo(int layout, BufferTarget target, string name)
        {
            Layout = layout;
            Target = target;
            Name = name;
        }
    }
}
