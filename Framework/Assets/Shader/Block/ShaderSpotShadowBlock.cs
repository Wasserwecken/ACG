using Framework.Extensions;
using OpenTK.Mathematics;
using System.Diagnostics;
using System.IO;

namespace Framework.Assets.Shader.Block
{
    public class ShaderSpotShadowBlock : ShaderBlockBase
    {
        [DebuggerDisplay("Strength: {Strength}, Area: {Area}")]
        public struct ShaderSpotShadow
        {
            public Vector4 Strength;
            public Vector4 Area;
            public Matrix4 Space;
        }

        public ShaderSpotShadow[] Shadows;


        /// <summary>
        /// 
        /// </summary>
        protected override void WriteBytes(BinaryWriter writer)
        {
            foreach (var shadow in Shadows)
            {
                writer.Write(shadow.Strength);
                writer.Write(shadow.Area);
                writer.Write(shadow.Space);
            }
        }
    }
}
