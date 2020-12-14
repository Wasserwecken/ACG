using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public static class Definitions
    {
        public static class Shader
        {
            public static class Attributes
            {
                public static class Vertices
                {
                    public const int LAYOUT = 0;
                    public const string NAME = "vertices";
                }
            
                public static class Normals
                {
                    public const int LAYOUT = 1;
                    public const string NAME = "normals";
                }
            
                public static class Colors
                {
                    public const int LAYOUT = 2;
                    public const string NAME = "colors";
                }   
            
                public static class UV
                {
                    public const int LAYOUT = 3;
                    public const string NAME = "uv";
                }
            }

            public static class Uniforms
            {
                public static class Time
                {
                    public const string NAME = "time";
                }
            }
        }

        public static class Buffer
        {
            public static class Vertices
            {
                public const int PAIRLENGTH = 3;
                public const bool NORMALIZED = false;
                public const VertexAttribPointerType POINTERTYPE = VertexAttribPointerType.Float;
            }

            public static class Normals
            {
                public const int PAIRLENGTH = 3;
                public const bool NORMALIZED = true;
                public const VertexAttribPointerType POINTERTYPE = VertexAttribPointerType.Float;
            }

            public static class Colors
            {
                public const int PAIRLENGTH = 4;
                public const bool NORMALIZED = false;
                public const VertexAttribPointerType POINTERTYPE = VertexAttribPointerType.Float;
            }

            public static class UV
            {
                public const int PAIRLENGTH = 2;
                public const bool NORMALIZED = false;
                public const VertexAttribPointerType POINTERTYPE = VertexAttribPointerType.Float;
            }
        }
    }
}
