using Framework.Extensions;
using OpenTK.Mathematics;
using System.IO;

namespace Framework.Assets.Shader.Block
{
    public class ShaderReflectionBlock : ShaderBlockBase
    {
        public struct ShaderReflectionProbe
        {
            public Vector4 Position;
            public Vector4 Area;
        }

        public ShaderReflectionProbe[] Probes;

        /// <summary>
        /// 
        /// </summary>
        protected override void WriteBytes(BinaryWriter writer)
        {
            foreach(var probe in Probes)
            {
                writer.Write(probe.Position);
                writer.Write(probe.Area);
            }
        }
    }
}
