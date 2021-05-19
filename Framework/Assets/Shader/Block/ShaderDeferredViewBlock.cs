using Framework.Extensions;
using OpenTK.Mathematics;
using System.IO;

namespace Framework.Assets.Shader.Block
{
    public class ShaderDeferredViewBlock : ShaderBlockBase
    {
        public Vector4 ViewPosition;
        public Vector4 ViewPort;
        public Vector2 Resolution;

        /// <summary>
        /// 
        /// </summary>
        protected override void WriteBytes(BinaryWriter writer)
        {
            writer.Write(ViewPosition);
            writer.Write(ViewPort);
            writer.Write(Resolution);
        }
    }
}
