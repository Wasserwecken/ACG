using OpenTK.Graphics.OpenGL;

namespace Framework.Assets.Verticies
{
    public class BufferIndicieAsset : BufferBaseAsset
    {
        /// <summary>
        /// 
        /// </summary>
        public BufferIndicieAsset(BufferUsageHint usageHint, uint[] indicies)
            : this(usageHint)
        {
            Data = indicies.ToBytes();
        }

        /// <summary>
        /// 
        /// </summary>
        public BufferIndicieAsset(BufferUsageHint usageHint)
            : base(usageHint, BufferTarget.ElementArrayBuffer, "Indicies", sizeof(uint)) { }
    }
}
