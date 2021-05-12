using Framework.Extensions;
using OpenTK.Mathematics;
using System.Diagnostics;
using System.IO;

namespace Framework.Assets.Shader.Block.Data
{
    public class ShaderPointLightBlock : ShaderBlockBase
    {
        [DebuggerDisplay("Color: {Color}, Position: {Position}")]
        public struct ShaderDirectionalLight
        {
            public Vector4 Color;
            public Vector4 Position;
        }

        public ShaderDirectionalLight[] Lights;


        /// <summary>
        /// 
        /// </summary>
        protected override void WriteBytes(BinaryWriter writer)
        {
            foreach(var light in Lights)
            {
                writer.Write(light.Color);
                writer.Write(light.Position);
            }
        }
    }
}
