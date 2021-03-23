using OpenTK.Graphics.OpenGL;

namespace Framework.Assets.Verticies.Attributes
{
    public interface IVertexAttribute
    {
        int Layout { get; }
        VertexAttribPointerType PointerType { get; }
        string Name { get; }
        int Dimension { get; }
        int ElementSize { get; }
        bool Normalize { get; }
        int ElementCount { get; }
        byte[] DataBytes { get; }

        void UpdateByteData();
    }
}
