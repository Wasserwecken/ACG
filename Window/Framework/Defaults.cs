using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OpenTK.Graphics.OpenGL;
using SharpGLTF.Schema2;

namespace Framework
{
    public static class Defaults
    {
        public static class Shader
        {
            public static class Source
            {
                public static ShaderSourceAsset VertexMesh { get; }
                public static ShaderSourceAsset VertexSkybox { get; }
                public static ShaderSourceAsset FragmentPBR { get; }
                public static ShaderSourceAsset FragmentSkybox { get; }
                public static ShaderSourceAsset FragmentBlinnPhong { get; }

                static Source()
                {
                    VertexMesh = ShaderSourceManager.Create(ShaderType.VertexShader, Path.Combine(Definitions.Directories.DefaultShader, "mesh.vert"));
                    VertexSkybox = ShaderSourceManager.Create(ShaderType.VertexShader, Path.Combine(Definitions.Directories.DefaultShader, "skybox.vert"));
                    
                    FragmentPBR = ShaderSourceManager.Create(ShaderType.FragmentShader, Path.Combine(Definitions.Directories.DefaultShader, "pbr.frag"));
                    FragmentSkybox = ShaderSourceManager.Create(ShaderType.FragmentShader, Path.Combine(Definitions.Directories.DefaultShader, "skybox.frag"));
                    FragmentBlinnPhong = ShaderSourceManager.Create(ShaderType.FragmentShader, Path.Combine(Definitions.Directories.DefaultShader, "blinnphong.frag"));
                }
            }

            public static class Program
            {
                public static ShaderProgramAsset MeshPBR { get; }
                public static ShaderProgramAsset MeshBlinnPhong { get; }

                static Program()
                {
                    MeshPBR = ShaderProgramManager.Create("MeshPBR", Source.VertexMesh, Source.FragmentPBR);
                    MeshBlinnPhong = ShaderProgramManager.Create("MeshBlinnPhong", Source.VertexMesh, Source.FragmentBlinnPhong);
                }
            }
        }

        public static class Material
        {
            public static MaterialAsset BlinnPhong { get; }

            static Material()
            {
                BlinnPhong = new MaterialAsset("BlinnPhong", Shader.Program.MeshBlinnPhong);
            }
        }

        public static class Vertex
        {
            public static class Primitive
            {
                public static VertexPrimitiveAsset Plane { get; }
                public static VertexPrimitiveAsset Cube { get; }
                public static VertexPrimitiveAsset Sphere { get; }

                static Primitive()
                {
                    var gltf = ModelRoot.Load(Path.Combine(Definitions.Directories.DefaultPrimitives, "primitives.glb"));
                    foreach(var gltfMesh in gltf.LogicalMeshes)
                    {
                        switch (gltfMesh.Name)
                        {
                            case "Sphere":
                                Sphere = GLTF2Manager.CreatePrimitive(gltfMesh.Primitives[0]);
                                break;

                            case "Cube":
                                Cube = GLTF2Manager.CreatePrimitive(gltfMesh.Primitives[0]);
                                break;

                            case "Plane":
                                Plane = GLTF2Manager.CreatePrimitive(gltfMesh.Primitives[0]);
                                break;
                        }
                    }
                }
            }
        }
    }
}
