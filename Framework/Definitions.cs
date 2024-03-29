﻿using OpenTK.Graphics.OpenGL;

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
            public const string DefaultImages = DefaultAssets + "Images/";
            public const string DefaultSkyboxes = DefaultImages + "Skyboxes/";
        }

        public static class Shader
        {
            public static class Uniform
            {
                public static string BaseColor => "BaseColor";
                public static string MREO => "MREO";
                public static string Normal => "Normal";
                public static string BaseColorMap => "BaseColorMap";
                public static string MetallicRoughnessMap => "MetallicRoughnessMap";
                public static string EmissiveMap => "EmissiveMap";
                public static string OcclusionMap => "OcclusionMap";
                public static string NormalMap => "NormalMap";
                public static string ReflectionMap => "ReflectionMap";
            }

            public class Attribute
            {
                public static class UInt
                {
                    public const int Size = 4;
                    public const VertexAttribPointerType PointerType = VertexAttribPointerType.UnsignedInt;
                }

                public static class Float
                {
                    public const int Size = 4;
                    public const VertexAttribPointerType PointerType = VertexAttribPointerType.Float;
                }

                public static class Vector2
                {
                    public const int Size = 8;
                    public const VertexAttribPointerType PointerType = VertexAttribPointerType.Float;
                }

                public static class Vector3
                {
                    public const int Size = 12;
                    public const VertexAttribPointerType PointerType = VertexAttribPointerType.Float;
                }

                public static class Vector4
                {
                    public const int Size = 16;
                    public const VertexAttribPointerType PointerType = VertexAttribPointerType.Float;
                }

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
