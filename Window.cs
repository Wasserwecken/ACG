using DefaultEcs;
using DefaultEcs.System;
using Framework;
using Framework.Assets.Materials;
using Framework.Assets.Shader.Block;
using Framework.ECS.Components.Light;
using Framework.ECS.Components.PostProcessing;
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
            _sceneComponents.Set(new InputComponent()
            {
                Keyboard = KeyboardState,
                Mouse = MouseState
            });
            _sceneComponents.Set(new AspectRatioComponent()
            {
                Width = nativeSettings.Size.X,
                Height = nativeSettings.Size.Y
            });
            _sceneComponents.Set(new ShadowBufferComponent()
            {
                Size = 4096,
                DirectionalBlock = new ShaderDirectionalShadowBlock(),
                PointBlock = new ShaderPointShadowBlock(),
                SpotBlock = new ShaderSpotShadowBlock(),
                FramebufferBuffer = Defaults.Framebuffer.CreateShadowBuffer()
            });
            _sceneComponents.Set(new ReflectionBufferComponent()
            {
                Size = 4096,
                ReflectionBlock = new ShaderReflectionBlock(),
                DeferredLightBuffer = Defaults.Framebuffer.CreateDeferredLightBuffer("ReflectionMap"),
                DeferredGBuffer = Defaults.Framebuffer.CreateDeferredGBuffer()
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
                new TransformRotatorSystem(_scene, _sceneComponents),
                new ReflectionProbeUpdateSystem(_scene, _sceneComponents)
            );

            _renderPipeline = new SequentialSystem<bool>(
                new ShaderTimeSystem(_scene, _sceneComponents),

                new DirectionalLightSystem(_scene, _sceneComponents),
                new PointLightSystem(_scene, _sceneComponents),
                new SpotLightSystem(_scene, _sceneComponents),

                new ShadowBufferPrepareSystem(_scene, _sceneComponents),
                new DirectionalShadowPassSystem(_scene, _sceneComponents),
                new PointShadowPassSystem(_scene, _sceneComponents),
                new SpotShadowPassSystem(_scene, _sceneComponents),
                new ShadowBufferSyncSystem(_scene, _sceneComponents),

                new ReflectionBufferPrepareSystem(_scene, _sceneComponents),
                new ReflectionDeferredPassSystem(_scene, _sceneComponents),
                new ReflectionForwardPassSystem(_scene, _sceneComponents),
                new ReflectionBufferSyncSystem(_scene, _sceneComponents),

                new CameraPrepareSystem(_scene, _sceneComponents),
                new CameraDeferredPassSystem(_scene, _sceneComponents),
                new CameraPostAmbientOcclusionSystem(_scene, _sceneComponents),
                new CameraForwardPassSystem(_scene, _sceneComponents),
                new CameraPostGlobalIllumination(_scene, _sceneComponents),
                //new CameraPostBloomSystem(_scene, _sceneComponents),
                new CameraPostTonemappingSystem(_scene, _sceneComponents),
                new CameraPublishSystem(_scene, _sceneComponents)

                , new FrameBufferDebugSystem(_scene, _sceneComponents)
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
            GLTF2Loader.Load(_scene, scenePath, Defaults.Shader.Program.MeshLitForward);


            var camera = _scene.CreateEntity();
            camera.Set(new TransformComponent(Vector3.UnitY * 4f, -Vector3.UnitX));
            camera.Set(new CameraControllerComponent() { MoveSpeed = 2f, LookSpeed = 1f });
            camera.Set(new PerspectiveCameraComponent() { FarClipping = 100f, NearClipping = 0.01f, FieldOfView = 90f, Skybox = Defaults.Texture.SkyboxCoast });
            camera.Set(new PostTonemappingComponent() { Exposure = 1f });
            camera.Set(new PostBloomComponent() { ThresholdStart = 0.7f, ThresholdEnd = 1.0f, Intensity = 1f, Samples = 5 });
            camera.Set(new PostAmbientOcclusionComponent() { Strength = 1f, Radius = 0.5f, Bias = 0.025f });
            camera.Set(new PostGlobalIllumination() { });
            camera.Set(new PrimitiveComponent() { IsShadowCaster = true, Material = Defaults.Material.PBR, Shader = Defaults.Shader.Program.MeshLitDeferredLight, Verticies = Defaults.Vertex.Mesh.Sphere[0] });


            var sunEntity = _scene.CreateEntity();
            sunEntity.Set(new TransformComponent(Vector3.Zero, -Vector3.UnitY.Rotate(-0.4f, Vector3.UnitX).Rotate(1f, Vector3.UnitY)));
            sunEntity.Set(new DirectionalLightComponent() { Color = Vector3.One * 5f, AmbientFactor = 0.003f });
            sunEntity.Set(new DirectionalShadowComponent() { Resolution = 2048, Strength = 1.0f, Width = 50, NearClipping = -25, FarClipping = +25 });
            sunEntity.Set(new TransformRotatorComponent() { Speed = 0.05f });

            var spotLight = _scene.CreateEntity();
            spotLight.Set(new TransformComponent(new Vector3(8.0f, 2f, 3f), new Vector3(0.5f, -0.1f, -1f)));
            spotLight.Set(new SpotLightComponent() { Color = new Vector3(1f, 1f, 0.6f) * 5f, AmbientFactor = 0.002f, InnerAngle = 0.3f, OuterAngle = 0.5f, Range = 10f });
            spotLight.Set(new SpotShadowComponent() { Resolution = 256, Strength = 1.0f, NearClipping = 0.01f });
            spotLight.Set(new TransformRotatorComponent() { Speed = 0.5f });

            var pointMaterial = new MaterialAsset("PointLight");
            pointMaterial.SetUniform("Albedo", Vector4.One);
            pointMaterial.SetUniform("MREO", new Vector4(0f, 1f, 3f, 0f));
            var rand = new Random();
            int pointLightCount = 0;
            for (int i = 0; i < pointLightCount; i++)
            {
                var pointLight = _scene.CreateEntity();
                var position = new Vector3((float)rand.NextDouble() * 2f - 1f, (float)rand.NextDouble() * 0.8f, (float)rand.NextDouble() * 0.25f - 0.125f);
                //var position = new Vector3(i - 1, 0.3f, 0f);
                pointLight.Set(new TransformComponent(position * 8.0f, Vector3.UnitZ, new Vector3(0.1f)));
                pointLight.Set(new PointLightComponent() { Color = new Vector3(1f, 1f, 0.6f) * 3f, AmbientFactor = 0.002f, Range = 8f });
                pointLight.Set(new PointShadowComponent() { Resolution = 1024, Strength = 1f, NearClipping = 0.01f });
                pointLight.Set(new PrimitiveComponent() { IsShadowCaster = false, Material = pointMaterial, Shader = Defaults.Shader.Program.MeshLitDeferredLight, Verticies = Defaults.Vertex.Mesh.Sphere[0] });
            }

            var probeMaterial = new MaterialAsset("ReflectionProbe");
            probeMaterial.SetUniform("Albedo", Vector4.One);
            probeMaterial.SetUniform("MREO", new Vector4(1f, 0f, 0f, 0f));
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    for (int z = 0; z < 3; z++)
                    {
                        var position = new Vector3(x - 1, y , z - 1) * new Vector3(9.5f, 3.5f, 3.3f) + new Vector3(-0.5f, 1.5f, -0.1f);
                        var reflectionProbe = _scene.CreateEntity();
                        reflectionProbe.Set(new TransformComponent(position));
                        reflectionProbe.Set(new ReflectionProbeComponent() { HasChanged = true, Resolution = 256, NearClipping = 0.01f, FarClipping = 30f, Skybox = Defaults.Texture.SkyboxCoast });
                        reflectionProbe.Set(new PrimitiveComponent() { IsShadowCaster = false, Material = probeMaterial, Shader = Defaults.Shader.Program.MeshLitDeferredLight, Verticies = Defaults.Vertex.Mesh.Sphere[0] });

                        //if (x == 1 && y == 0 && z == 1) reflectionProbe.Set(new ReflectionProbeUpdateComponent());
                    }
                }
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