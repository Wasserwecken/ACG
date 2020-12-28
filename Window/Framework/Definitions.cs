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
            
                public static class UV
                {
                    public const int LAYOUT = 2;
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
                    public const string WORLD = "LocalToWorldSpace";
                    public const string WORLD_ROTATION = "LocalToWorldRotationSpace";
                    public const string VIEW = "LocalToViewSpace";
                    public const string VIEW_ROTATION = "LocalToViewRotationSpace";
                    public const string PROJECTION = "LocalToProjectionSpace";
                }

                public static class Normal
                {
                    public const string WORLD = "NormalToWorld";
                    public const string PROJECTION = "NormalToView";
                }

                public static class View
                {
                    public const string POSITION = "ViewPositon";
                }

                public static class Light
                {
                    public static class Ambient
                    {
                        public const string COLOR = "LightAmbientColor";
                    }

                    public static class Directional
                    {
                        public const string COUNT = "LightDirectionalCount";
                        public const string COLOR = "LightDirectionalColor";
                        public const string DIRECTION = "LightDirectionalDirection";
                    }
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

            public static class UV
            {
                public const int PAIRLENGTH = 2;
                public const bool NORMALIZED = false;
                public const VertexAttribPointerType POINTERTYPE = VertexAttribPointerType.Float;
            }
        }
    }
}
