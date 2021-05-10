using DefaultEcs;
using DefaultEcs.System;
using Framework;
using Framework.Assets.Framebuffer;
using Framework.Assets.Shader.Block.Data;
using Framework.Assets.Textures;
using Framework.ECS.Components.Light;
using Framework.ECS.Components.Scene;
using Framework.ECS.Components.Transform;
using Framework.ECS.GLTF2;
using Framework.ECS.Systems.Hierarchy;
using Framework.ECS.Systems.Render;
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

        private readonly Stopwatch _renderWatch;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameSettings"></param>
        /// <param name="nativeSettings"></param>
        public Window(GameWindowSettings gameSettings, NativeWindowSettings nativeSettings)
            : base(gameSettings, nativeSettings)
        {
            _renderWatch = new Stopwatch();

            _scene = new World();
            _sceneComponents = _scene.CreateEntity();
            _sceneComponents.Set(new TimeComponent());
            _sceneComponents.Set(new InputComponent() { Keyboard = KeyboardState, Mouse = MouseState });
            _sceneComponents.Set(new AspectRatioComponent() { Width = nativeSettings.Size.X, Height = nativeSettings.Size.Y });
            _sceneComponents.Set(new DirectionalLightCollectionComponent()
            {
                Data = new ShaderDirectionalLight[0],
                ShadowSpacer = new TextureSpace(4096, new Vector3(0f, 0f, 1f)),
                ShadowBuffer = new FramebufferAsset("DirectionalShadowPass")
                {
                    Width = 4096,
                    Height = 4096,

                    DrawMode = DrawBufferMode.None,
                    ReadMode = ReadBufferMode.None,

                    Textures = new List<TextureRenderAsset>()
                    {
                        new TextureRenderAsset("DirectionalShadowMap")
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
            _sceneComponents.Set(new PointLightCollectionComponent()
            {
                Data = new ShaderPointLight[0],
                ShadowSpacer = new TextureSpace(4096, new Vector3(0f, 0f, 1f)),
                ShadowBuffer = new FramebufferAsset("PointShadowPass")
                {
                    Width = 4096,
                    Height = 4096,

                    DrawMode = DrawBufferMode.None,
                    ReadMode = ReadBufferMode.None,

                    Textures = new List<TextureRenderAsset>()
                    {
                        new TextureRenderAsset("PointShadowMap")
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


            _fixedPipeline = new SequentialSystem<bool>(
                new FixedTimeSystem(_scene, _sceneComponents)
            );

            _framePipeline = new SequentialSystem<bool>(
                new TotalTimeSystem(_scene, _sceneComponents),
                new FrameTimeSystem(_scene, _sceneComponents),

                new EntityHierarchySystem(_scene, _sceneComponents),
                new TransformHierarchySystem(_scene, _sceneComponents),

                new CameraControllerSystem(_scene, _sceneComponents),
                new TransformRotatorSystem(_scene, _sceneComponents)
            );

            _renderPipeline = new SequentialSystem<bool>(
                new TimeSyncSystem(_scene, _sceneComponents),
                new TextureSyncSystem(_scene, _sceneComponents),

                new DirectionalLightSystem(_scene, _sceneComponents),
                new DirectionalShadowPassSystem(_scene, _sceneComponents),
                new PointLightSystem(_scene, _sceneComponents),
                new PointShadowPassSystem(_scene, _sceneComponents),

                new ForwardPassSystemOLD(_scene, _sceneComponents),
                new FrameBufferDebugSystem(_scene, _sceneComponents)
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

            var rand = new Random();
            for (int i = 0; i < 0; i++)
            {
                var pointLight = _scene.CreateEntity();
                //var position = new Vector3( i - 1, 0.3f, (float)rand.NextDouble() * 0.25f - 0.125f);
                var position = new Vector3( i - 1, 0.3f, 0f);
                pointLight.Set(new TransformComponent(position * 8.0f));
                //pointLight.Set(new TransformComponent(new Vector3(0f, 2f, 0f)));
                pointLight.Set(new PointLightComponent() { Color = new Vector3(1f, 1f, 0.5f) * 2, AmbientFactor = 0.001f, Range = 8f });
                pointLight.Set(new PointShadowComponent() { Resolution = 512, Strength = 1f, NearClipping = 0.01f });
            }
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

            _renderWatch.Restart();

            _framePipeline.Update(true);
            _renderPipeline.Update(true);

            var time = _renderWatch.ElapsedMilliseconds;
            Title = $"{1000 / MathF.Max(float.Epsilon, time):F1} fps / {time} ms";
            
            Context.SwapBuffers();
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