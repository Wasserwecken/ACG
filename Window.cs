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
using System.Collections.Generic;
using System.Linq;

namespace Window
{
    public class Window : GameWindow
    {
        private readonly World _scene;
        private readonly SequentialSystem<bool> _fixedPipeline;
        private readonly SequentialSystem<bool> _framePipeline;
        private readonly SequentialSystem<bool> _renderPipeline;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameSettings"></param>
        /// <param name="nativeSettings"></param>
        public Window(GameWindowSettings gameSettings, NativeWindowSettings nativeSettings)
            : base(gameSettings, nativeSettings)
        {
            var sceneComponents = _scene.CreateEntity();
            sceneComponents.Set(new TimeComponent());
            sceneComponents.Set(new RenderDataComponent());
            sceneComponents.Set(new InputComponent() { Keyboard = KeyboardState, Mouse = MouseState });
            sceneComponents.Set(new AspectRatioComponent() { Width = nativeSettings.Size.X, Height = nativeSettings.Size.Y });


            _fixedPipeline = new SequentialSystem<bool>(
                new FixedTimeSystem(_scene)
            );

            _framePipeline = new SequentialSystem<bool>(
                new TotalTimeSystem(_scene),
                new FrameTimeSystem(_scene),

                new EntityHierarchySystem(_scene),
                new TransformHierarchySystem(_scene),

                new CameraControllerSystem(_scene)
            );

            _renderPipeline = new SequentialSystem<bool>(
                    new RenderHierarchySystem(),
                    new TimeSyncSystem(_scene),
                    new LightSyncSystem(_scene),
                    new TextureSyncSystem(_scene),
                    new PrimitiveSyncSystem(_scene),
                    new RenderSystem(_scene)
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
            _sceneEntities.AddRange(GLTF2Loader.Load(scenePath, Defaults.Shader.Program.MeshBlinnPhong));


            if (!_sceneEntities.Any(f => f.Components.Has<PerspectiveCameraComponent>()))
            {
                var camera = Defaults.Entity.Camera;
                camera.Components.Add(new CameraControllerComponent() { MoveSpeed = 2f, LookSpeed = 1f });
                _sceneEntities.Add(camera);
            }
            if (!_sceneEntities.Any(f => f.Components.Has<DirectionalLightComponent>()))
            {
                _sceneEntities.Add(new Entity("Sun",
                    new TransformComponent() { Forward = -Vector3.UnitY.Rotate(1f, Vector3.UnitX).Rotate(1f, Vector3.UnitY) },
                    new DirectionalLightComponent() { Color = Vector3.One, AmbientFactor = 0.005f }
                ));
            }


            var foo = new FramebufferAsset("Test")
            {
                TextureTargets = new List<FrameBufferTextureAsset>()
                {
                    new FrameBufferTextureAsset("color"),
                },
                StorageTargets = new List<FramebufferStorageAsset>()
                {
                    new FramebufferStorageAsset("depth") { DataType = RenderbufferStorage.DepthComponent },
                    new FramebufferStorageAsset("stencil") { DataType = RenderbufferStorage.DepthStencil },
                }
            };
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

            _framePipeline.Update(true);
            _renderPipeline.Update(true);

            Context.SwapBuffers();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);

            var aspectRatioComponent = _scene.Get<AspectRatioComponent>()[0];
            aspectRatioComponent.Width = e.Width;
            aspectRatioComponent.Height = e.Height;
        }
    }
}