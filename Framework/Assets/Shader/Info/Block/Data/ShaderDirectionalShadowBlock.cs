using Framework.Extensions;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Framework.Assets.Shader.Info.Block.Data
{
    public class ShaderDirectionalShadowBlock : ShaderBlockBase
    {
        public struct ShaderDirectionalShadow
        {
            public Vector4 Strength;
            public Vector4 Area;
            public Matrix4 Space;
        }

        public ShaderDirectionalShadow[] Shadows;


        /// <summary>
        /// 
        /// </summary>
        public override void WriteBytes(BinaryWriter writer)
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
