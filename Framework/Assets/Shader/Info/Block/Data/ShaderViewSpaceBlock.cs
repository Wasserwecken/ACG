using Framework.Extensions;
using OpenTK.Mathematics;
using System.IO;

namespace Framework.Assets.Shader.Block.Data
{
    public class ShaderViewSpaceBlock : ShaderBlockBase
    {
        public Matrix4 WorldToView;
        public Matrix4 WorldToProjection;
        public Matrix4 WorldToViewRotation;
        public Matrix4 WorldToProjectionRotation;
        public Vector4 ViewPosition;
        public Vector4 ViewDirection;
        public Matrix4 ViewProjection;

        /// <summary>
        /// 
        /// </summary>
        protected override void WriteBytes(BinaryWriter writer)
        {
            writer.Write(WorldToView);
            writer.Write(WorldToProjection);
            writer.Write(WorldToViewRotation);
            writer.Write(WorldToProjectionRotation);
            writer.Write(ViewPosition);
            writer.Write(ViewDirection);
            writer.Write(ViewProjection);
        }
    }
}
