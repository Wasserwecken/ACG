using Framework.Extensions;
using OpenTK.Mathematics;
using System.Diagnostics;
using System.IO;

namespace Framework.Assets.Shader.Info.Block.Data
{
    public class ShaderPointShadowBlock : ShaderBlockBase
    {
        [DebuggerDisplay("Strength: {Strength}, Area: {Area}")]
        public struct ShaderPointShadow
        {
            public Vector4 Strength;
            public Vector4 Area;
        }

        public ShaderPointShadow[] Shadows;


        /// <summary>
        /// 
        /// </summary>
        protected override void WriteBytes(BinaryWriter writer)
        {
            foreach (var shadow in Shadows)
            {
                writer.Write(shadow.Strength);
                writer.Write(shadow.Area);
            }
        }
    }
}
