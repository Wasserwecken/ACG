using Framework.ECS.Components.Light;
using Framework.Extensions;
using OpenTK.Mathematics;
using System.Diagnostics;
using System.IO;

namespace Framework.Assets.Shader.Block
{
    public class ShaderDirectionalLightBlock : ShaderBlockBase
    {
        [DebuggerDisplay("Color: {Color}, Direction: {Direction}")]
        public struct ShaderDirectionalLight
        {
            public Vector4 Color;
            public Vector4 Direction;
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
                writer.Write(light.Direction);
            }
        }
    }
}
