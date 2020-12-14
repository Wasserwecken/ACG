using Framework;
using Framework.Mesh;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Diagnostics;

namespace Window
{
    public class Window : GameWindow
    {
        private GameWindowSettings _gameSettings;
        private NativeWindowSettings _nativeSettings;

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
        }

        ShaderProgram program;
        Texture2D tex1, tex2;
        Material mat;
        MeshObject mesh;
        Stopwatch watch;


        /// <summary>
        /// 
        /// </summary>
        protected override void OnLoad()
        {
            base.OnLoad();

            watch = new Stopwatch();
            watch.Start();

            var vert = new ShaderSource(ShaderType.VertexShader, "Assets/shader.vert");
            var frag = new ShaderSource(ShaderType.FragmentShader, "Assets/shader.frag");
            program = new ShaderProgram(vert, frag);
            Console.WriteLine(program.Log);

            tex1 = new Texture2D("Assets/wall.jpg");
            tex2 = new Texture2D("Assets/awesomeface.png");

            mat = new Material(program);
            mat.SetUniform("texture1", tex1);
            mat.SetUniform("texture2", tex2);



            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);


            var vertices = new float[] {
                //Position         
                 0.5f,  0.5f, 0.0f, // top right
                 0.5f, -0.5f, 0.0f, // bottom right
                -0.5f, -0.5f, 0.0f, // bottom left
                -0.5f,  0.5f, 0.0f  // top left
            };
            var uv = new float[] {
                //Texture coordinates
                1.0f, 1.0f, // top right
                1.0f, 0.0f, // bottom right
                0.0f, 0.0f, // bottom left
                0.0f, 1.0f  // top left
            };
            var indices = new uint[] {  // note that we start from 0!
                0, 1, 3,   // first triangle
                1, 2, 3    // second triangle
            };


            mesh = new MeshObject(BufferUsageHint.StaticDraw);
            mesh.AddAttribute(new VertexAttribute<float>(0, "vertices", 3, false, vertices));
            mesh.AddAttribute(new VertexAttribute<float>(1, "uv", 2, false, uv));
            mesh.AddIndicies(indices);
            mesh.Prepare();
            mesh.PushToGPU();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);


            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                this.Close();
            }
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            mat.Use();
            mat.SetUniform("time", (float)watch.Elapsed.TotalSeconds);

            mesh.Draw();

            Context.SwapBuffers();
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnUnload()
        {
            base.OnUnload();

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
