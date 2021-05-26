using System.Collections.Generic;
using System.IO;
using DefaultEcs;
using Framework.Assets.Framebuffer;
using Framework.Assets.Materials;
using Framework.Assets.Shader;
using Framework.Assets.Textures;
using Framework.Assets.Verticies;
using Framework.ECS.Components.Relation;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Transform;
using Framework.ECS.GLTF2.Assets;
using Framework.ECS.Systems.Render.OpenGL;
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
                public static ShaderSourceAsset FragmentLitDeferredBuffer { get; }
                public static ShaderSourceAsset FragmentLitDeferredLight { get; }
                public static ShaderSourceAsset FragmentSkybox { get; }
                public static ShaderSourceAsset FragmentShadow { get; }

                public static ShaderSourceAsset FragmentFrameBuffer { get; }
                public static ShaderSourceAsset FragmentTonemapping { get; }
                public static ShaderSourceAsset FragmentBloomSelect { get; }
                public static ShaderSourceAsset FragmentBloomMerge { get; }
                public static ShaderSourceAsset FragmentGaussianBlur { get; }

                static Source()
                {
                    VertexScreenQuad = new ShaderSourceAsset(ShaderType.VertexShader, Path.Combine(Definitions.Directories.DefaultShader, "ScreenQuad.vert"));
                    VertexMesh = new ShaderSourceAsset(ShaderType.VertexShader, Path.Combine(Definitions.Directories.DefaultShader, "mesh.vert"));
                    VertexSkybox = new ShaderSourceAsset(ShaderType.VertexShader, Path.Combine(Definitions.Directories.DefaultShader, "skybox.vert"));

                    FragmentLitBlinnPhong = new ShaderSourceAsset(ShaderType.FragmentShader, Path.Combine(Definitions.Directories.DefaultShader, "LitBlinnPhong.frag"));
                    FragmentLitDeferredBuffer = new ShaderSourceAsset(ShaderType.FragmentShader, Path.Combine(Definitions.Directories.DefaultShader, "LitDeferredBuffer.frag"));
                    FragmentLitDeferredLight = new ShaderSourceAsset(ShaderType.FragmentShader, Path.Combine(Definitions.Directories.DefaultShader, "LitDeferredLight.frag"));
                    FragmentSkybox = new ShaderSourceAsset(ShaderType.FragmentShader, Path.Combine(Definitions.Directories.DefaultShader, "skybox.frag"));
                    FragmentShadow = new ShaderSourceAsset(ShaderType.FragmentShader, Path.Combine(Definitions.Directories.DefaultShader, "Shadow.frag"));
                
                    FragmentFrameBuffer = new ShaderSourceAsset(ShaderType.FragmentShader, Path.Combine(Definitions.Directories.DefaultShader, "FrameBuffer.frag"));
                    FragmentTonemapping = new ShaderSourceAsset(ShaderType.FragmentShader, Path.Combine(Definitions.Directories.DefaultShader, "PostTonemapping.frag"));
                    FragmentBloomSelect = new ShaderSourceAsset(ShaderType.FragmentShader, Path.Combine(Definitions.Directories.DefaultShader, "PostBloomSelect.frag"));
                    FragmentBloomMerge = new ShaderSourceAsset(ShaderType.FragmentShader, Path.Combine(Definitions.Directories.DefaultShader, "PostBloomMerge.frag"));
                    FragmentGaussianBlur = new ShaderSourceAsset(ShaderType.FragmentShader, Path.Combine(Definitions.Directories.DefaultShader, "PostGaussianBlur.frag"));
                }
            }

            public static class Program
            {
                public static ShaderProgramAsset FrameBuffer { get; }
                public static ShaderProgramAsset Skybox { get; }
                public static ShaderProgramAsset Shadow { get; }
                public static ShaderProgramAsset MeshBlinnPhong { get; }
                public static ShaderProgramAsset MeshLitDeferredBuffer { get; }
                public static ShaderProgramAsset MeshLitDeferredLight { get; }

                public static ShaderProgramAsset PostTonemapping { get; }
                public static ShaderProgramAsset PostBloomSelect { get; }
                public static ShaderProgramAsset PostBloomMerge { get; }
                public static ShaderProgramAsset PostGaussianBlur { get; }

                static Program()
                {
                    FrameBuffer = new ShaderProgramAsset("FrameBuffer", Source.VertexScreenQuad, Source.FragmentFrameBuffer);
                    Skybox = new ShaderProgramAsset("Skybox", Source.VertexSkybox, Source.FragmentSkybox);
                    Shadow = new ShaderProgramAsset("Shadow", Source.VertexMesh, Source.FragmentShadow);
                    MeshBlinnPhong = new ShaderProgramAsset("MeshBlinnPhong", Source.VertexMesh, Source.FragmentLitBlinnPhong);
                    MeshLitDeferredBuffer = new ShaderProgramAsset("MeshLitDeferredBuffer", Source.VertexMesh, Source.FragmentLitDeferredBuffer);
                    MeshLitDeferredLight = new ShaderProgramAsset("MeshLitDeferredLight", Source.VertexScreenQuad, Source.FragmentLitDeferredLight);

                    PostTonemapping = new ShaderProgramAsset("PostTonemapping", Source.VertexScreenQuad, Source.FragmentTonemapping);
                    PostBloomSelect = new ShaderProgramAsset("PostBloomSelect", Source.VertexScreenQuad, Source.FragmentBloomSelect);
                    PostBloomMerge = new ShaderProgramAsset("PostBloomMerge", Source.VertexScreenQuad, Source.FragmentBloomMerge);
                    PostGaussianBlur = new ShaderProgramAsset("PostGaussianBlur", Source.VertexScreenQuad, Source.FragmentGaussianBlur);
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
                White = new Texture2DAsset("DefaultWhite") { Image = new ImageAsset("White") { Data = Color4.White.ToShort() } };
                Gray = new Texture2DAsset("DefaultGray") { Image = new ImageAsset("Gray") { Data = Color4.Gray.ToShort() } };
                Black = new Texture2DAsset("DefaultBlack") { Image = new ImageAsset("Black") { Data = Color4.Black.ToShort() } };
                Normal = new Texture2DAsset("DefaultNormal") { Image = new ImageAsset("Normal") { Data = new Color4(0.5f, 0.5f, 1f, 1f).ToShort() } };

                GPUSync.Push(White);
                GPUSync.Push(Gray);
                GPUSync.Push(Black);
                GPUSync.Push(Normal);

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

                GPUSync.Push(SkyboxCoast);
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

        public static class Framebuffer
        {
            public static FramebufferAsset CreateShadowBuffer() => new FramebufferAsset("ShadowBuffer")
            {
                DrawMode = DrawBufferMode.None,
                ReadMode = ReadBufferMode.None,

                Textures = new List<TextureRenderAsset>()
                {
                    new TextureRenderAsset("ShadowMap")
                    {
                        Attachment = FramebufferAttachment.DepthAttachment,

                        InternalFormat = PixelInternalFormat.DepthComponent,
                        Format = PixelFormat.DepthComponent,
                        PixelType = PixelType.Float
                    }
                }
            };

            public static FramebufferAsset CreateDeferredLightBuffer(string resultName) => new FramebufferAsset("DeferredLightBuffer")
            {
                DrawMode = DrawBufferMode.None,
                ReadMode = ReadBufferMode.None,

                DrawTargets = new DrawBuffersEnum[] { DrawBuffersEnum.ColorAttachment0 },
                Storages = new List<FramebufferStorageAsset>()
                {
                    new FramebufferStorageAsset("DeferredDepth")
                    {
                        Attachment = FramebufferAttachment.DepthStencilAttachment,
                        Target = RenderbufferTarget.Renderbuffer,
                        DataType = RenderbufferStorage.Depth24Stencil8
                    }
                },
                Textures = new List<TextureRenderAsset>()
                {
                    new TextureRenderAsset(resultName)
                    {
                        Attachment = FramebufferAttachment.ColorAttachment0,
                        InternalFormat = PixelInternalFormat.Rgb16f,
                        Format = PixelFormat.Rgb,
                        PixelType = PixelType.Float,
                    }
                }
            };

            public static FramebufferAsset CreateDeferredGBuffer() => new FramebufferAsset("DeferredGBuffer")
            {
                DrawMode = DrawBufferMode.None,
                ReadMode = ReadBufferMode.None,

                DrawTargets = new DrawBuffersEnum[]
                {
                    DrawBuffersEnum.ColorAttachment0,
                    DrawBuffersEnum.ColorAttachment1,
                    DrawBuffersEnum.ColorAttachment2,
                    DrawBuffersEnum.ColorAttachment3,
                    DrawBuffersEnum.ColorAttachment4,
                    DrawBuffersEnum.ColorAttachment5,
                },
                Storages = new List<FramebufferStorageAsset>()
                {
                    new FramebufferStorageAsset("DeferredDepth")
                    {
                        Attachment = FramebufferAttachment.DepthStencilAttachment,
                        Target = RenderbufferTarget.Renderbuffer,
                        DataType = RenderbufferStorage.Depth24Stencil8
                    }
                },
                Textures = new List<TextureRenderAsset>()
                {
                    new TextureRenderAsset("DeferredPosition")
                    {
                        Attachment = FramebufferAttachment.ColorAttachment0,
                        InternalFormat = PixelInternalFormat.Rgba16f,
                        Format = PixelFormat.Rgba,
                        PixelType = PixelType.Float,
                    },
                    new TextureRenderAsset("DeferredAlbedo")
                    {
                        Attachment = FramebufferAttachment.ColorAttachment1,
                        InternalFormat = PixelInternalFormat.Rgb16f,
                        Format = PixelFormat.Rgb,
                        PixelType = PixelType.Float
                    },
                    new TextureRenderAsset("DeferredNormalSurface")
                    {
                        Attachment = FramebufferAttachment.ColorAttachment2,
                        InternalFormat = PixelInternalFormat.Rgb16f,
                        Format = PixelFormat.Rgb,
                        PixelType = PixelType.Float
                    },
                    new TextureRenderAsset("DeferredNormalTexture")
                    {
                        Attachment = FramebufferAttachment.ColorAttachment3,
                        InternalFormat = PixelInternalFormat.Rgb16f,
                        Format = PixelFormat.Rgb,
                        PixelType = PixelType.Float
                    },
                    new TextureRenderAsset("DeferredMRO")
                    {
                        Attachment = FramebufferAttachment.ColorAttachment4,
                        InternalFormat = PixelInternalFormat.Rgb16,
                        Format = PixelFormat.Rgb,
                        PixelType = PixelType.Float
                    },
                    new TextureRenderAsset("DeferredEmission")
                    {
                        Attachment = FramebufferAttachment.ColorAttachment5,
                        InternalFormat = PixelInternalFormat.Rgb16,
                        Format = PixelFormat.Rgb,
                        PixelType = PixelType.Float
                    }
                }
            };
        }
    }
}
