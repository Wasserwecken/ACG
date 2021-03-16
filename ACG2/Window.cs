﻿using Framework;
using Framework.ECS;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Scene;
using Framework.ECS.GLTF2;
using Framework.ECS.Systems;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Project.ECS.Components;
using Project.ECS.Systems;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Window
{
    public class Window : GameWindow
    {
        private GameWindowSettings _gameSettings;
        private NativeWindowSettings _nativeSettings;
        private readonly Stopwatch _totalWatch;

        private readonly List<IComponent> _sceneComponents;
        private readonly List<Entity> _sceneEntities;

        private readonly List<ISystem> _frameSystems;
        private readonly List<ISystem> _updateSystems;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameSettings"></param>
        /// <param name="nativeSettings"></param>
        public Window(GameWindowSettings gameSettings, NativeWindowSettings nativeSettings)
            : base(gameSettings, nativeSettings)
        {
            _gameSettings = gameSettings;
            _nativeSettings = nativeSettings;

            _sceneEntities = new List<Entity>();
            _sceneComponents = new List<IComponent>()
            {
                new AspectRatioComponent() { Width = _nativeSettings.Size.X, Height = _nativeSettings.Size.Y },
                new InputComponent() { Keyboard = KeyboardState, Mouse = MouseState },
                new TimeComponent(),
                new RenderDataComponent(),
                new SkyboxComponent()
                {
                    Shader = Default.Shader.Program.Skybox,
                    Material = Default.Material.Skybox,
                    Mesh = Default.Vertex.Mesh.Cube
                }
            };

            _totalWatch = new Stopwatch();
            _totalWatch.Start();

            _updateSystems = new List<ISystem>()
            {
            };

            _frameSystems = new List<ISystem>()
            {
                new CameraControllerSystem(),

                new EntityHierarchySystem(),
                new TransformHierarchySystem(),
                new RenderHierarchySystem(),
                new PrimitiveSyncSystem(),
                new LightSyncSystem(),
                new RenderSystem()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnLoad()
        {
            base.OnLoad();

            Console.WriteLine(GL.GetString(StringName.Version));
            Console.WriteLine(GL.GetString(StringName.ShadingLanguageVersion));
            Console.WriteLine(GL.GetString(StringName.Renderer));
            Console.WriteLine(GL.GetError());

            //var scenePath = "./Assets/foo.glb";
            var scenePath = "./Assets/Samples/DamagedHelmet/glTF-Binary/DamagedHelmet.glb";
            //var scenePath = "./Assets/Samples/Buggy/glTF-Binary/Buggy.glb";
            //var scenePath = "./Assets/Samples/TextureCoordinateTest/glTF-Binary/TextureCoordinateTest.glb";
            //var scenePath = "./Assets/Samples/Sponza/glTF/Sponza.gltf";


            _sceneEntities.AddRange(GLTF2Loader.Load(scenePath, Default.Shader.Program.MeshUnlit));
            if (!_sceneEntities.Any(f => f.HasAnyComponents(typeof(PerspectiveCameraComponent))))
            {
                var camera = Default.Entity.Camera;
                camera.Components.Add(new CameraControllerComponent() { MoveSpeed = 2f, LookSpeed = 1f });
                _sceneEntities.Add(camera);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (KeyboardState.IsKeyDown(Keys.Escape)) Close();

            var timeComponent = (TimeComponent)_sceneComponents.First(component => component is TimeComponent);
            timeComponent.Total = _totalWatch.ElapsedMilliseconds / 1000f;
            timeComponent.TotalSin = MathF.Sin(timeComponent.Total);
            timeComponent.DeltaFrame = (float)args.Time;

            foreach (var system in _updateSystems)
                system.Update(_sceneEntities, _sceneComponents);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            var timeComponent = (TimeComponent)_sceneComponents.First(component => component is TimeComponent);
            timeComponent.Total = _totalWatch.ElapsedMilliseconds / 1000f;
            timeComponent.TotalSin = MathF.Sin(timeComponent.Total);
            timeComponent.DeltaFixed = (float)args.Time;

            foreach (var system in _frameSystems)
                system.Update(_sceneEntities, _sceneComponents);

            Context.SwapBuffers();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);

            var aspectRatioComponent = (AspectRatioComponent)_sceneComponents.First(component => component is AspectRatioComponent);
            aspectRatioComponent.Width = e.Width;
            aspectRatioComponent.Height = e.Height;
        }
    }
}