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
                    public const string NAME = "BufferVertex";
                }
            
                public static class Normals
                {
                    public const int LAYOUT = 1;
                    public const string NAME = "BufferNormal";
                }
            
                public static class Colors
                {
                    public const int LAYOUT = 2;
                    public const string NAME = "BufferColor";
                }   
            
                public static class UV
                {
                    public const int LAYOUT = 3;
                    public const string NAME = "BufferUV";
                }
            }

            public static class Uniforms
            {
                public static class Time
                {
                    public const string TOTAL = "TimeTotal";
                    public const string DELTA = "TimeDelta";
                }

                public static class Space
                {
                    public const string WORLD = "WorldSpace";
                    public const string VIEW = "ViewSpace";
                    public const string PROJECTION = "ProjectionSpace";
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
