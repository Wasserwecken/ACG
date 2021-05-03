using DefaultEcs;
using DefaultEcs.System;
using Framework;
using Framework.Assets.Framebuffer;
using Framework.ECS.Components.Light;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Scene;
using Framework.ECS.Components.Transform;
using Framework.ECS.GLTF2;
using Framework.ECS.Systems.Hierarchy;
using Framework.ECS.Systems.Render;
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


            _fixedPipeline = new SequentialSystem<bool>(
                new FixedTimeSystem(_scene, _sceneComponents)
            );

            _framePipeline = new SequentialSystem<bool>(
                new TotalTimeSystem(_scene, _sceneComponents),
                new FrameTimeSystem(_scene, _sceneComponents),

                new EntityHierarchySystem(_scene, _sceneComponents),
                new TransformHierarchySystem(_scene, _sceneComponents),

                new CameraControllerSystem(_scene, _sceneComponents)
            );

            _renderPipeline = new SequentialSystem<bool>(
                new TimeSyncSystem(_scene, _sceneComponents),
                new LightSyncSystem(_scene, _sceneComponents),
                new TextureSyncSystem(_scene, _sceneComponents),
                new PrimitiveSyncSystem(_scene, _sceneComponents),

                new ShadowPassSystem(_scene, _sceneComponents),
                new ForwardPassSystemOLD(_scene, _sceneComponents)
            ); ;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnLoad()
        {
            base.OnLoad();

            //var scenePath = "./Assets/foo.glb";
            var scenePath = "./Assets/Samples/DamagedHelmet/glTF-Binary/DamagedHelmet.glb";
            //var scenePath = "./Assets/Samples/Sponza/glTF/Sponza.gltf";
            GLTF2Loader.Load(_scene, scenePath, Defaults.Shader.Program.MeshBlinnPhong);

            var cameraEntity = Defaults.Entities.Camera(_scene);
            cameraEntity.Set(new CameraControllerComponent() { MoveSpeed = 2f, LookSpeed = 1f });

            var sunEntity = _scene.CreateEntity();
            sunEntity.Set(new TransformComponent(Vector3.Zero, -Vector3.UnitY.Rotate(1f, Vector3.UnitX).Rotate(1f, Vector3.UnitY)));
            sunEntity.Set(new DirectionalLightComponent() { Color = Vector3.One, AmbientFactor = 0.005f });
            sunEntity.Set(new ShadowCasterComponent() { Resolution = 1024, NearClipping = -500, FarClipping = +500 });
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
            Context.SwapBuffers();

            var time = _renderWatch.ElapsedMilliseconds;
            Title = $"{1000 / MathF.Max(float.Epsilon, time):F1} fps / {time} ms";
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