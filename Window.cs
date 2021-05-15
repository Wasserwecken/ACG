﻿using DefaultEcs;
using DefaultEcs.System;
using Framework;
using Framework.Assets.Framebuffer;
using Framework.Assets.Materials;
using Framework.Assets.Shader.Block;
using Framework.Assets.Textures;
using Framework.ECS.Components.Light;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Scene;
using Framework.ECS.Components.Transform;
using Framework.ECS.GLTF2;
using Framework.ECS.Systems.Hierarchy;
using Framework.ECS.Systems.Render;
using Framework.ECS.Systems.Render.Pipeline;
using Framework.ECS.Systems.RenderPipeline;
using Framework.ECS.Systems.Sync;
using Framework.ECS.Systems.Time;
using Framework.Extensions;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Project.ECS.Components;
using Project.ECS.Systems;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Window
{
    public class Window : GameWindow
    {
        private readonly World _scene;
        private readonly Entity _sceneComponents;
        private readonly SequentialSystem<bool> _fixedPipeline;
        private readonly SequentialSystem<bool> _framePipeline;
        private readonly SequentialSystem<bool> _renderPipeline;

        private readonly Stopwatch _frameWatch;
        private readonly Stopwatch _renderWatch;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameSettings"></param>
        /// <param name="nativeSettings"></param>
        public Window(GameWindowSettings gameSettings, NativeWindowSettings nativeSettings)
            : base(gameSettings, nativeSettings)
        {
            _frameWatch = new Stopwatch();
            _renderWatch = new Stopwatch();

            _scene = new World();
            _sceneComponents = _scene.CreateEntity();
            _sceneComponents.Set(new TimeComponent());
            _sceneComponents.Set(new InputComponent() { Keyboard = KeyboardState, Mouse = MouseState });
            _sceneComponents.Set(new AspectRatioComponent() { Width = nativeSettings.Size.X, Height = nativeSettings.Size.Y });
            _sceneComponents.Set(new ShadowBufferComponent()
            {
                DirectionalBlock = new ShaderDirectionalShadowBlock(),
                PointBlock = new ShaderPointShadowBlock(),
                SpotBlock = new ShaderSpotShadowBlock(),
                TextureAtlas = new TextureSpace(4096, new Vector3(0f, 0f, 1f)),
                FramebufferBuffer = new FramebufferAsset("ShadowBuffer")
                {
                    Width = 4096,
                    Height = 4096,

                    DrawMode = DrawBufferMode.None,
                    ReadMode = ReadBufferMode.None,

                    Textures = new List<TextureRenderAsset>()
                    {
                        new TextureRenderAsset("ShadowMap")
                        {
                            Attachment = FramebufferAttachment.DepthAttachment,
                            Width = 4096,
                            Height = 4096,

                            InternalFormat = PixelInternalFormat.DepthComponent,
                            Format = PixelFormat.DepthComponent,
                            PixelType = PixelType.Float
                        }
                    }
                }
            });
            _sceneComponents.Set(new ReflectionBufferComponent()
            {
                ReflectionBlock = new ShaderReflectionBlock(),
                TextureAtlas = new TextureSpace(4096, new Vector3(0f, 0f, 1f)),
                DeferredBuffer = new FramebufferAsset("DeferredRelfectionBuffer")
                {
                    Target = FramebufferTarget.Framebuffer,
                    Width = 4096,
                    Height = 4096,

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
                        new FramebufferStorageAsset("ReflectionDeferredDepth")
                        {
                            Attachment = FramebufferAttachment.DepthAttachment,
                            Width = 4096,
                            Height = 4096,

                            Target = RenderbufferTarget.Renderbuffer,
                            DataType = RenderbufferStorage.DepthComponent
                        }
                    },
                    Textures = new List<TextureRenderAsset>()
                    {
                        new TextureRenderAsset("DeferredPosition")
                        {
                            Attachment = FramebufferAttachment.ColorAttachment0,
                            Width = 4096,
                            Height = 4096,

                            InternalFormat = PixelInternalFormat.Rgba16,
                            Format = PixelFormat.Rgba,
                            PixelType = PixelType.Float,
                        },
                        new TextureRenderAsset("DeferredAlbedo")
                        {
                            Attachment = FramebufferAttachment.ColorAttachment1,
                            Width = 4096,
                            Height = 4096,

                            InternalFormat = PixelInternalFormat.Rgb16,
                            Format = PixelFormat.Rgb,
                            PixelType = PixelType.Float
                        },
                        new TextureRenderAsset("DeferredNormalSurface")
                        {
                            Attachment = FramebufferAttachment.ColorAttachment2,
                            Width = 4096,
                            Height = 4096,

                            InternalFormat = PixelInternalFormat.Rgb16,
                            Format = PixelFormat.Rgb,
                            PixelType = PixelType.Float
                        },
                        new TextureRenderAsset("DeferredNormalTexture")
                        {
                            Attachment = FramebufferAttachment.ColorAttachment3,
                            Width = 4096,
                            Height = 4096,

                            InternalFormat = PixelInternalFormat.Rgb16,
                            Format = PixelFormat.Rgb,
                            PixelType = PixelType.Float
                        },
                        new TextureRenderAsset("DeferredMRO")
                        {
                            Attachment = FramebufferAttachment.ColorAttachment4,
                            Width = 4096,
                            Height = 4096,

                            InternalFormat = PixelInternalFormat.Rgb16,
                            Format = PixelFormat.Rgb,
                            PixelType = PixelType.Float
                        },
                        new TextureRenderAsset("DeferredEmission")
                        {
                            Attachment = FramebufferAttachment.ColorAttachment5,
                            Width = 4096,
                            Height = 4096,

                            InternalFormat = PixelInternalFormat.Rgb16,
                            Format = PixelFormat.Rgb,
                            PixelType = PixelType.Float
                        }
                    }
                }
            });


            _fixedPipeline = new SequentialSystem<bool>(
                new FixedTimeSystem(_scene, _sceneComponents)
            );

            _framePipeline = new SequentialSystem<bool>(
                new TotalTimeSystem(_scene, _sceneComponents),
                new FrameTimeSystem(_scene, _sceneComponents),

                new EntityHierarchySystem(_scene, _sceneComponents),
                new TransformHierarchySystem(_scene, _sceneComponents),
                new PrimitiveSpaceSystem(_scene, _sceneComponents),

                new CameraControllerSystem(_scene, _sceneComponents),
                new TransformRotatorSystem(_scene, _sceneComponents)
            );

            _renderPipeline = new SequentialSystem<bool>(
                new ShaderTimeSystem(_scene, _sceneComponents),

                new ShadowBufferPrepareSystem(_scene, _sceneComponents),
                new DirectionalLightSystem(_scene, _sceneComponents),
                new DirectionalShadowPassSystem(_scene, _sceneComponents),
                new PointLightSystem(_scene, _sceneComponents),
                new PointShadowPassSystem(_scene, _sceneComponents),
                new SpotLightSystem(_scene, _sceneComponents),
                new SpotShadowPassSystem(_scene, _sceneComponents),
                new ShadowBufferSyncSystem(_scene, _sceneComponents),

                new ReflectionBufferPrepareSystem(_scene, _sceneComponents),
                new ReflectionDeferredPassSystem(_scene, _sceneComponents),
                new ReflectionBufferSyncSystem(_scene, _sceneComponents),

                new ForwardPassSystemOLD(_scene, _sceneComponents)
                ,new FrameBufferDebugSystem(_scene, _sceneComponents)
            );
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnLoad()
        {
            base.OnLoad();

            //var scenePath = "./Assets/foo.glb";
            //var scenePath = "./Assets/Samples/DamagedHelmet/glTF-Binary/DamagedHelmet.glb";
            var scenePath = "./Assets/Samples/Sponza/glTF/Sponza.gltf";
            //var scenePath = "./Assets/Samples/BoomBoxWithAxes/glTF/BoomBoxWithAxes.gltf";
            GLTF2Loader.Load(_scene, scenePath, Defaults.Shader.Program.MeshBlinnPhong);

            var cameraEntity = Defaults.Entities.Camera(_scene);
            cameraEntity.Set(new CameraControllerComponent() { MoveSpeed = 2f, LookSpeed = 1f });


            var sunEntity = _scene.CreateEntity();
            sunEntity.Set(new TransformComponent(Vector3.Zero, -Vector3.UnitY.Rotate(-0.4f, Vector3.UnitX).Rotate(1f, Vector3.UnitY)));
            sunEntity.Set(new DirectionalLightComponent() { Color = Vector3.One, AmbientFactor = 0.005f });
            sunEntity.Set(new DirectionalShadowComponent() { Resolution = 2048, Strength = 1.0f, Width = 50, NearClipping = -25, FarClipping = +25 });
            //sunEntity.Set(new TransformRotatorComponent() { Speed = 0.05f });

            var sphereMaterial = new MaterialAsset("Foo");
            sphereMaterial.SetUniform("Albedo", Vector4.One);
            sphereMaterial.SetUniform("MREO", new Vector4(1f, 0f, 0f, 0f));

            var sphere = _scene.CreateEntity();
            sphere.Set(new TransformComponent(new Vector3(0f, 2f, 0f)));
            sphere.Set(new PrimitiveComponent()
            {
                IsShadowCaster = false,
                Material = sphereMaterial,
                Shader = Defaults.Shader.Program.MeshBlinnPhong,
                Verticies = Defaults.Vertex.Mesh.Sphere[0]
            });

            var spotLight = _scene.CreateEntity();
            spotLight.Set(new TransformComponent(new Vector3(8.0f, 2f, 3f), new Vector3(0.5f, -0.1f, -1f)));
            spotLight.Set(new SpotLightComponent() { Color = new Vector3(1f, 1f, 0.6f) * 3f, AmbientFactor = 0.001f, InnerAngle = 0.3f, OuterAngle = 0.5f, Range = 10f });
            spotLight.Set(new SpotShadowComponent() { Resolution = 256, Strength = 1.0f, NearClipping = 0.01f });

            var rand = new Random();
            int pointLightCount = 1;
            for (int i = 0; i < pointLightCount; i++)
            {
                var pointLight = _scene.CreateEntity();
                //var position = new Vector3((float)rand.NextDouble() * 2f - 1f, (float)rand.NextDouble() * 0.8f, (float)rand.NextDouble() * 0.25f - 0.125f);
                var position = new Vector3( i - 1, 0.3f, 0f);
                pointLight.Set(new TransformComponent(position * 8.0f));
                //pointLight.Set(new TransformComponent(new Vector3(0f, 2f, 0f)));
                pointLight.Set(new PointLightComponent() { Color = new Vector3(1f, 1f, 0.6f) * 3f, AmbientFactor = 0.001f, Range = 8f });
                pointLight.Set(new PointShadowComponent() { Resolution = 1024, Strength = 1f, NearClipping = 0.01f });
            }

            var reflectionProbe = _scene.CreateEntity();
            reflectionProbe.Set(new TransformComponent(new Vector3(0f, 1f, 0f)));
            reflectionProbe.Set(new ReflectionProbeComponent() { HasChanged = true, Resolution = 4096, NearClipping = 0.01f, FarClipping = 30f });
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            _fixedPipeline.Update(true);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            _frameWatch.Restart();
            _framePipeline.Update(true);
            var frameTime = _frameWatch.Elapsed.TotalMilliseconds;

            _renderWatch.Restart();
            _renderPipeline.Update(true);
            var renderTime = _frameWatch.Elapsed.TotalMilliseconds;

            Context.SwapBuffers();
            
            var total = frameTime + renderTime;
            Title = $"{1000 / Math.Max(double.Epsilon, total):F1} fps | Total: {total:F2} ms | Update: {frameTime:F2} ms | Render {renderTime:F2}";
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);

            ref var aspectRatio = ref _sceneComponents.Get<AspectRatioComponent>();
            aspectRatio.Width = e.Width;
            aspectRatio.Height = e.Height;
        }
    }
}