using Framework.Assets.Shader.Info.Block;
using OpenTK.Graphics.OpenGL;
using System.IO;

namespace Framework.ECS.Components.Scene
{
    public struct TimeComponent
    {
        public float Total;
        public float TotalSin;
        public float DeltaFrame;
        public float DeltaFixed;
    }
}
