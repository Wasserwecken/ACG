using Framework;
using Framework.Assets.Shader;
using Framework.ECS;
using Framework.ECS.Components;
using Framework.ECS.Components.Relation;
using Framework.ECS.Components.Scene;
using Framework.ECS.GLTF2;
using Framework.ECS.Systems;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
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

        private readonly List<IComponent> _sceneComponents;
        private readonly List<Entity> _sceneEntities;

        private readonly Stopwatch _totalWatch;
        private readonly Stopwatch _deltaWatch;

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

            _sceneComponents = new List<IComponent>();
            _sceneEntities = new List<Entity>();

            _totalWatch = new Stopwatch();
            _totalWatch.Start();
            _deltaWatch = new Stopwatch();
            _deltaWatch.Start();

            _updateSystems = new List<ISystem>()
            {
                new ParentChildSystem(),
                new TransformSystem()
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

            //var scenePath = "./Assets/Samples/TextureCoordinateTest/glTF-Binary/TextureCoordinateTest.glb";
            //var scenePath = "./Assets/Samples/Sponza/glTF/Sponza.gltf";
            var scenePath = "./Assets/foo.glb";

            _sceneComponents.Add(new AspectRatioComponent() { Width = _nativeSettings.Size.X, Height = _nativeSettings.Size.Y });
            _sceneComponents.Add(new TimeComponent());

            _sceneEntities.Add(Defaults.Entities.Camera);
            _sceneEntities.AddRange(GLTF2Loader.Load(scenePath, Defaults.Shader.Program.MeshUnlit));
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
            timeComponent.Delta = _deltaWatch.ElapsedMilliseconds / 1000f;
            timeComponent.TotalSin = MathF.Sin(timeComponent.Total);
            timeComponent.TotalSin01 = timeComponent.TotalSin * 0.5f + 0.5f;
            _deltaWatch.Restart();

            foreach (var system in _updateSystems)
                system.Update(_sceneEntities, _sceneComponents);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);


            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //_scene.Render();

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