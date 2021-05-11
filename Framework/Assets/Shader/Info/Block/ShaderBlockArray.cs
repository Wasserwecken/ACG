using System.Diagnostics;
using System.Runtime.InteropServices;
using ACG.Framework.Assets;
using Framework.Assets.Shader.Block.Data;
using OpenTK.Graphics.OpenGL;

namespace Framework.Assets.Shader.Block
{
    [DebuggerDisplay("Handle: {Handle}, Name: {Name}, Count: {Data?.Length}")]
    public class ShaderBlockArray<TBlockType> : IShaderBlock where TBlockType : struct
    {
        public static int BlockSize = Marshal.SizeOf(typeof(TBlockType));
        public int Handle { get; private set; }
        public bool IsGlobal { get; private set; }
        public string Name { get; private set; }
        public BufferRangeTarget Target { get; private set; }
        public BufferUsageHint UsageHint { get; set; }
        public TBlockType[] Data;

        /// <summary>
        /// 
        /// </summary>
        public ShaderBlockArray(bool isGlobal, BufferRangeTarget target, BufferUsageHint usageHint)
        {
            Handle = -1;
            Name = typeof(TBlockType).Name;
            Data = default;
            Target = target;
            IsGlobal = isGlobal;
            UsageHint = usageHint;

            AssetRegister.ShaderBlocks.Add(this);
        }

        /// <summary>
        /// 
        /// </summary>
        public void PushToGPU()
        {
            if (Handle < 0)
                Handle = GL.GenBuffer();

            GL.BindBuffer((BufferTarget)Target, Handle);
            GL.BufferData((BufferTarget)Target, BlockSize * Data.Length, Data, UsageHint);
        }
    }
}
