using Framework.Extensions;
using OpenTK.Mathematics;
using System.IO;

namespace Framework.Assets.Shader.Block
{
    public class ShaderSpotLightBlock : ShaderBlockBase
    {
        public struct ShaderSpotLight
        {
            public Vector4 Color;
            public Vector4 Position;
            public Vector4 Direction;
        }

        public ShaderSpotLight[] Lights;

        /// <summary>
        /// 
        /// </summary>
        protected override void WriteBytes(BinaryWriter writer)
        {
            foreach(var light in Lights)
            {
                writer.Write(light.Color);
                writer.Write(light.Position);
                writer.Write(light.Direction);
            }
        }
    }
}
