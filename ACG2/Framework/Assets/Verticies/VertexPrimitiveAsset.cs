using System.Diagnostics;
using OpenTK.Graphics.OpenGL;

namespace Framework.Assets.Verticies
{
    [DebuggerDisplay("Type: {Type}, Mode: {Mode}")]
    public class VertexPrimitiveAsset
    {
        public int Handle { get; set; }
        public BufferArrayAsset ArrayBuffer { get; }
        public BufferIndicieAsset IndicieBuffer { get; }
        public PolygonMode Mode { get; set; }
        public PrimitiveType Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public VertexPrimitiveAsset(VertexAttributeAsset[] attributes, uint[] indicies, BufferUsageHint usageHint)
            : this(new BufferArrayAsset(usageHint, attributes), new BufferIndicieAsset(usageHint, indicies)) { }

        /// <summary>
        /// 
        /// </summary>
        public VertexPrimitiveAsset()
            : this(new BufferArrayAsset(BufferUsageHint.StaticDraw), new BufferIndicieAsset(BufferUsageHint.StaticDraw)) { }

        /// <summary>
        /// 
        /// </summary>
        public VertexPrimitiveAsset(BufferArrayAsset arrayBuffer, BufferIndicieAsset indicieBuffer)
        {
            ArrayBuffer = arrayBuffer;
            IndicieBuffer = indicieBuffer;

            Mode = PolygonMode.Fill;
            Type = PrimitiveType.Triangles;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Draw()
        {
            GL.BindVertexArray(Handle);
            GL.PolygonMode(MaterialFace.FrontAndBack, Mode);

            if (IndicieBuffer != null)
                GL.DrawElements(Type, IndicieBuffer.Indicies.Length, DrawElementsType.UnsignedInt, 0);
            //else
            //    GL.DrawArrays(Type, 0, ArrayBuffer.ElementCount);
        }
    }
}
