using OpenTK.Graphics.OpenGL;

namespace Framework.Assets.Verticies
{
    public class BufferIndicieAsset : BufferBaseAsset
    {
        public override int ElementSize => sizeof(uint);
        public override int ElementCount => Indicies.Length;
        public uint[] Indicies { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public BufferIndicieAsset(BufferUsageHint usageHint, uint[] indicies)
            : this(usageHint)
        {
            Indicies = indicies;
        }

        /// <summary>
        /// 
        /// </summary>
        public BufferIndicieAsset(BufferUsageHint usageHint)
            : base(usageHint, BufferTarget.ElementArrayBuffer, "Indicies") { }
    }
}
