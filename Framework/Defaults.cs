using System.Collections.Generic;
using System.IO;
using DefaultEcs;
using Framework.Assets.Materials;
using Framework.Assets.Shader;
using Framework.Assets.Textures;
using Framework.Assets.Verticies;
using Framework.ECS.Components.Relation;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Transform;
using Framework.ECS.GLTF2.Assets;
using Framework.Extensions;
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
                public static ShaderSourceAsset VertexScreenQuad { get; }
                public static ShaderSourceAsset VertexMesh { get; }
                public static ShaderSourceAsset VertexSkybox { get; }

                public static ShaderSourceAsset FragmentLitBlinnPhong { get; }
                public static ShaderSourceAsset FragmentSkybox { get; }
                public static ShaderSourceAsset FragmentShadow { get; }
                public static ShaderSourceAsset FragmentFrameBuffer { get; }

                static Source()
                {
                    VertexScreenQuad = new ShaderSourceAsset(ShaderType.VertexShader, Path.Combine(Definitions.Directories.DefaultShader, "ScreenQuad.vert"));
                    VertexMesh = new ShaderSourceAsset(ShaderType.VertexShader, Path.Combine(Definitions.Directories.DefaultShader, "mesh.vert"));
                    VertexSkybox = new ShaderSourceAsset(ShaderType.VertexShader, Path.Combine(Definitions.Directories.DefaultShader, "skybox.vert"));

                    FragmentLitBlinnPhong = new ShaderSourceAsset(ShaderType.FragmentShader, Path.Combine(Definitions.Directories.DefaultShader, "LitBlinnPhong.frag"));
                    FragmentSkybox = new ShaderSourceAsset(ShaderType.FragmentShader, Path.Combine(Definitions.Directories.DefaultShader, "skybox.frag"));
                    FragmentShadow = new ShaderSourceAsset(ShaderType.FragmentShader, Path.Combine(Definitions.Directories.DefaultShader, "Shadow.frag"));
                    FragmentFrameBuffer = new ShaderSourceAsset(ShaderType.FragmentShader, Path.Combine(Definitions.Directories.DefaultShader, "FrameBuffer.frag"));
                }
            }

            public static class Program
            {
                public static ShaderProgramAsset FrameBuffer { get; }
                public static ShaderProgramAsset Skybox { get; }
                public static ShaderProgramAsset Shadow { get; }
                public static ShaderProgramAsset MeshBlinnPhong { get; }

                static Program()
                {
                    FrameBuffer = new ShaderProgramAsset("FrameBuffer", Source.VertexScreenQuad, Source.FragmentFrameBuffer);
                    Skybox = new ShaderProgramAsset("Skybox", Source.VertexSkybox, Source.FragmentSkybox);
                    Shadow = new ShaderProgramAsset("Shadow", Source.VertexMesh, Source.FragmentShadow);
                    MeshBlinnPhong = new ShaderProgramAsset("MeshBlinnPhong", Source.VertexMesh, Source.FragmentLitBlinnPhong);
                }
            }
        }

        public static class Texture
        {
            public static Texture2DAsset White { get; }
            public static Texture2DAsset Gray { get; }
            public static Texture2DAsset Black { get; }
            public static Texture2DAsset Normal { get; }
            public static TextureCubeAsset SkyboxCoast { get; }

            static Texture()
            {
                White = new Texture2DAsset("Default") { Image = new ImageAsset("White") { Data = Color4.White.ToShort() } };
                Gray = new Texture2DAsset("Default") { Image = new ImageAsset("White") { Data = Color4.Gray.ToShort() } };
                Black = new Texture2DAsset("Default") { Image = new ImageAsset("White") { Data = Color4.Black.ToShort() } };
                Normal = new Texture2DAsset("Default") { Image = new ImageAsset("Normal") { Data = Color4.Blue.ToShort() } };

                SkyboxCoast = new TextureCubeAsset("DefaultCoast")
                {
                    Images = new ImageAsset[]
                    {
                        Helper.LoadImage(Definitions.Directories.DefaultSkyboxes + "Coast/right.jpg"),
                        Helper.LoadImage(Definitions.Directories.DefaultSkyboxes + "Coast/left.jpg"),
                        Helper.LoadImage(Definitions.Directories.DefaultSkyboxes + "Coast/top.jpg"),
                        Helper.LoadImage(Definitions.Directories.DefaultSkyboxes + "Coast/bottom.jpg"),
                        Helper.LoadImage(Definitions.Directories.DefaultSkyboxes + "Coast/front.jpg"),
                        Helper.LoadImage(Definitions.Directories.DefaultSkyboxes + "Coast/back.jpg"),
                    }
                };
            }
        }

        public static class Material
        {
            public static MaterialAsset Skybox { get; }
            public static MaterialAsset Shadow { get; }
            public static MaterialAsset PBR { get; }

            static Material()
            {
                Shadow = new MaterialAsset("Shadow");

                Skybox = new MaterialAsset("Skybox");
                Skybox.CullingMode = CullFaceMode.Front;
                Skybox.DepthTest = DepthFunction.Lequal;
                Skybox.SetUniform("ReflectionMap", Texture.SkyboxCoast);

                PBR = new MaterialAsset("Default PBR");
                PBR.SetUniform("BaseColor", new Vector4(0.8f, 0.8f, 0.9f, 1.0f));
                PBR.SetUniform("MREO", new Vector4(0.0f, 0.5f, 0.0f, 1.0f));
                PBR.SetUniform("Normal", 1f);
            }
        }

        public static class Vertex
        {
            public static class Mesh
            {
                public static List<VertexPrimitiveAsset> Plane { get; }
                public static List<VertexPrimitiveAsset> Cube { get; }
                public static List<VertexPrimitiveAsset> Sphere { get; }

                static Mesh()
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
            public static Entity Camera(World world)
            {
                var entity = world.CreateEntity();

                entity.Set(TransformComponent.Default);
                entity.Set(new PerspectiveCameraComponent()
                {
                    ClearColor = new Vector4(0.2f),
                    ClearMask = ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit,
                    FarClipping = 100f,
                    NearClipping = 0.01f,
                    FieldOfView = 90f
                });

                foreach(var primitive in Vertex.Mesh.Cube)
                {
                    var subEntity = world.CreateEntity();
                    subEntity.Set(TransformComponent.Default);
                    subEntity.Set(new ChildComponent(entity));
                    subEntity.Set(new PrimitiveComponent()
                    {
                        IsShadowCaster = false,
                        Shader = Shader.Program.Skybox,
                        Material = Material.Skybox,
                        Verticies = primitive
                    });
                }

                return entity;
            }
        }
    }
}
