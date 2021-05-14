using Framework.Assets.Shader.Block;
using System.Diagnostics;

namespace Framework.ECS.Components.Light
{
    [DebuggerDisplay("Resolution: {Resolution}, Strength: {StrengthCount}")]
    public struct SpotShadowComponent
    {
        public ShaderViewSpaceBlock ViewSpaceBlock;

        public int Resolution;
        public float Strength;
        public float NearClipping;
    }
}
