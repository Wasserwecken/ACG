using Framework.Extensions;
using OpenTK.Mathematics;
using System.IO;

namespace Framework.Assets.Shader.Block.Data
{
    public class ShaderPrimitiveSpaceBlock : ShaderBlockBase
    {
        public Matrix4 LocalToWorld;
        public Matrix4 LocalToWorldRotation;


        /// <summary>
        /// 
        /// </summary>
        protected override void WriteBytes(BinaryWriter writer)
        {
            writer.Write(LocalToWorld);
            writer.Write(LocalToWorldRotation);
        }
    }
}
