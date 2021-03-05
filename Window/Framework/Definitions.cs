using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public static class Definitions
    {
        public static class Directories
        {
            public const string AssetPath = "./Assets/";
            public const string DefaultAssets = AssetPath + "Defaults/";
            public const string DefaultShader = DefaultAssets + "Shader/";
            public const string DefaultPrimitives = DefaultAssets + "Primitives/";
        }

        public static class Buffer
        {
            public static class VertexAttribute
            {
                public static class Position
                {
                    public const string Name = "Position";
                    public const int Layout = 0;
                    public const int Size = 12;
                    public const bool Normalize = false;
                    public const VertexAttribPointerType PointerType = VertexAttribPointerType.Float;
                }

                public static class Normal
                {
                    public const string Name = "Normal";
                    public const int Layout = 1;
                    public const int Size = 12;
                    public const bool Normalize = false;
                    public const VertexAttribPointerType PointerType = VertexAttribPointerType.Float;
                }

                public static class Tangent
                {
                    public const string Name = "Tangent";
                    public const int Layout = 2;
                    public const int Size = 16;
                    public const bool Normalize = false;
                    public const VertexAttribPointerType PointerType = VertexAttribPointerType.Float;
                }

                public static class UV
                {
                    public const string Name = "UV";
                    public const int Layout = 3;
                    public const int Size = 8;
                    public const bool Normalize = false;
                    public const VertexAttribPointerType PointerType = VertexAttribPointerType.Float;
                }

                public static class Color
                {
                    public const string Name = "Color";
                    public const int Layout = 4;
                    public const int Size = 16;
                    public const bool Normalize = false;
                    public const VertexAttribPointerType PointerType = VertexAttribPointerType.Float;
                }
            }
        }
    }
}
