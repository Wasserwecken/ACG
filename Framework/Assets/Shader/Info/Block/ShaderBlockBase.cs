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

        private readonly MemoryStream _stream;
        private readonly BinaryWriter _writer;


        /// <summary>
        /// 
        /// </summary>
        public ShaderBlockBase()
        {
            Handle = -1;
            Name = GetType().Name;
            Target = BufferRangeTarget.ShaderStorageBuffer;
            UsageHint = BufferUsageHint.DynamicDraw;

            _stream = new MemoryStream();
            _writer = new BinaryWriter(_stream);
        }

        /// <summary>
        /// 
        /// </summary>
        public byte[] GetBytes()
        {
            _stream.Position = 0;
            WriteBytes(_writer);
            return _stream.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        protected abstract void WriteBytes(BinaryWriter writer);
    }
}
