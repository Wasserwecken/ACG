using Framework;
using Framework.Assets.Framebuffer;
using Framework.ECS;
using Framework.ECS.Components.Light;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Scene;
using Framework.ECS.Components.Transform;
using Framework.ECS.GLTF2;
using Framework.ECS.Pipeline;
using Framework.ECS.Systems;
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
        private List<Entity> _sceneEntities;
        private List<IComponent> _sceneComponents;

        private readonly Pipeline _fixedPipeline;
        private readonly Pipeline _framePipeline;
        private readonly Pipeline _renderPipeline;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameSettings"></param>
        /// <param name="nativeSettings"></param>
        public Window(GameWindowSettings gameSettings, NativeWindowSettings nativeSettings)
            : base(gameSettings, nativeSettings)
        {
            _fixedPipeline = new Pipeline()
            {
                Systems = new List<ISystem>()
                {
                    new FixedTimeSystem(),
                },
            };

            _framePipeline = new Pipeline()
            {
                Systems = new List<ISystem>()
                {
                    new TotalTimeSystem(),
                    new FrameTimeSystem(),
                    new CameraControllerSystem(),
                    new EntityHierarchySystem(),
                    new TransformHierarchySystem(),
                }
            };

            _renderPipeline = new Pipeline()
            {
                Systems = new List<ISystem>()
                {
                    new TimeSyncSystem(),
                    new LightSyncSystem(),
                    new TextureSyncSystem(),
                    new PrimitiveSyncSystem(),
                    new RenderSystem()
                }
            };

            _sceneComponents = new List<IComponent>()
            {
                new TimeComponent(),
                new AspectRatioComponent() { Width = nativeSettings.Size.X, Height = nativeSettings.Size.Y },
                new InputComponent() { Keyboard = KeyboardState, Mouse = MouseState }
            };

            _sceneEntities = new List<Entity>()
            {
                new Entity("Sky",
                    new TransformComponent(),
                    new SkyboxComponent()
                    {
                        Shader = Default.Shader.Program.Skybox,
                        Material = Default.Material.Skybox,
                        Mesh = Default.Vertex.Mesh.Cube
                    }
                )
            };
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
            _sceneEntities.AddRange(GLTF2Loader.Load(scenePath, Default.Shader.Program.MeshBlinnPhong));


            if (!_sceneEntities.Any(f => f.Components.Has<PerspectiveCameraComponent>()))
            {
                var camera = Default.Entity.Camera;
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

            foreach (var entitiy in _sceneEntities.Where(f => f.Components.Has<MeshComponent>()))
                entitiy.Components.Add(new TurntableComponent() { Speed = 1f });


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

            _fixedPipeline.Process(_sceneEntities, _sceneComponents);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            _framePipeline.Process(_sceneEntities, _sceneComponents);
            _renderPipeline.Process(_sceneEntities, _sceneComponents);

            Context.SwapBuffers();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);

            var aspectRatioComponent = _sceneComponents.Get<AspectRatioComponent>();
            aspectRatioComponent.Width = e.Width;
            aspectRatioComponent.Height = e.Height;
        }
    }
}