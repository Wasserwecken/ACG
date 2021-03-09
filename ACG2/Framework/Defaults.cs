using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Framework.Assets.Materials;
using Framework.Assets.Shader;
using Framework.Assets.Verticies;
using Framework.ECS;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Transform;
using Framework.ECS.GLTF2.Assets;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
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
                public static ShaderSourceAsset FragmentUnlit { get; }
                public static ShaderSourceAsset FragmentLitPBR { get; }
                public static ShaderSourceAsset FragmentSkybox { get; }
                public static ShaderSourceAsset FragmentBlinnPhong { get; }

                static Source()
                {
                    VertexMesh = new ShaderSourceAsset(ShaderType.VertexShader, Path.Combine(Definitions.Directories.DefaultShader, "mesh.vert"));
                    VertexSkybox = new ShaderSourceAsset(ShaderType.VertexShader, Path.Combine(Definitions.Directories.DefaultShader, "skybox.vert"));

                    FragmentUnlit = new ShaderSourceAsset(ShaderType.FragmentShader, Path.Combine(Definitions.Directories.DefaultShader, "Unlit.frag"));
                    FragmentLitPBR = new ShaderSourceAsset(ShaderType.FragmentShader, Path.Combine(Definitions.Directories.DefaultShader, "LitPBR.frag"));
                    FragmentSkybox = new ShaderSourceAsset(ShaderType.FragmentShader, Path.Combine(Definitions.Directories.DefaultShader, "skybox.frag"));
                    FragmentBlinnPhong = new ShaderSourceAsset(ShaderType.FragmentShader, Path.Combine(Definitions.Directories.DefaultShader, "blinnphong.frag"));
                }
            }

            public static class Program
            {
                public static ShaderProgramAsset MeshUnlit { get; }
                public static ShaderProgramAsset MeshPBR { get; }
                public static ShaderProgramAsset MeshBlinnPhong { get; }

                static Program()
                {
                    MeshUnlit = new ShaderProgramAsset("MeshUnlit", Source.VertexMesh, Source.FragmentUnlit);
                    MeshPBR = new ShaderProgramAsset("MeshLitPBR", Source.VertexMesh, Source.FragmentLitPBR);
                    MeshBlinnPhong = new ShaderProgramAsset("MeshBlinnPhong", Source.VertexMesh, Source.FragmentBlinnPhong);
                }
            }
        }

        public static class Materials
        {
            public static MaterialAsset PBR { get; }

            static Materials()
            {
                PBR = new MaterialAsset("Default");
                PBR.SetUniform("BaseColor", new Vector4(0.8f, 0.8f, 0.9f, 1.0f));
                PBR.SetUniform("MREO", new Vector4(0.0f, 0.5f, 0.0f, 1.0f));
                PBR.SetUniform("Normal", 1f);
            }
        }

        public static class Vertex
        {
            public static class Primitive
            {
                public static MeshAsset Plane { get; }
                public static MeshAsset Cube { get; }
                public static MeshAsset Sphere { get; }

                static Primitive()
                {
                    var gltf = ModelRoot.Load(Path.Combine(Definitions.Directories.DefaultPrimitives, "primitives.glb"));
                    foreach(var gltfMesh in gltf.LogicalMeshes)
                    {
                        switch (gltfMesh.Name)
                        {
                            case "Sphere":
                                Sphere = CreatorMeshAsset.Create(gltfMesh);
                                break;

                            case "Cube":
                                Cube = CreatorMeshAsset.Create(gltfMesh);
                                break;

                            case "Plane":
                                Plane = CreatorMeshAsset.Create(gltfMesh);
                                break;
                        }
                    }
                }
            }
        }

        public static class Entities
        {
            public static Entity Camera { get; }

            static Entities()
            {
                Camera = new Entity("Camera");
                Camera.Components.Add(new TransformComponent()
                {
                    Position = new Vector3(0, 0, -5),
                    Forward = new Vector3(0, 0, 1)
                });
                Camera.Components.Add(new PerspectiveCameraComponent()
                {
                    ClearColor = new Vector4(0.2f),
                    ClearMask = ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit,
                    FarClipping = 100f,
                    NearClipping = 0.01f,
                    FieldOfView = 90f
                });
            }
        }
    }
}
