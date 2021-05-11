using Framework.Assets.Shader.Info.Block;
using OpenTK.Graphics.OpenGL;
using System.IO;

namespace Framework
{
    public abstract class ShaderBlockBase
    {
        public int Handle { get; set; }
        public string Name { get; }
        public BufferRangeTarget Target { get; set; }
        public BufferUsageHint UsageHint { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public ShaderBlockBase()
        {
            Handle = -1;
            Name = GetType().Name;
            Target = BufferRangeTarget.ShaderStorageBuffer;
            UsageHint = BufferUsageHint.DynamicDraw;
        }

        /// <summary>
        /// 
        /// </summary>
        public abstract void WriteBytes(BinaryWriter writer);
    }
}
