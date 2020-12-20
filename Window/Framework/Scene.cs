using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Framework
{
    public class Scene
    {
        private Stopwatch _watchTotal;
        private Stopwatch _watchDelta;

        private float TimeDelta { get; set; }
        private float TimeTotal => (float)_watchTotal.Elapsed.TotalSeconds;

        ShaderProgram program;
        Texture2D tex1, tex2;
        Material mat;
        MeshObject mesh;
        PerspectiveCameraComponent camera;
        TransformComponent cameraTransform;
        TransformComponent meshTransform;

        /// <summary>
        /// 
        /// </summary>
        public Scene()
        {
            _watchTotal = new Stopwatch();
            _watchTotal.Start();

            _watchDelta = new Stopwatch();


            var vert = new ShaderSource(ShaderType.VertexShader, "Assets/shader.vert");
            var frag = new ShaderSource(ShaderType.FragmentShader, "Assets/shader.frag");
            program = new ShaderProgram(vert, frag);
            Console.WriteLine(program.Log);


            meshTransform = new TransformComponent();
            mesh = Load3DHelper.Load("Assets/ape.obj")[0];
            mesh.PushToGPU();


            tex1 = new Texture2D("Assets/wall.jpg");
            tex1.PushToGPU();

            tex2 = new Texture2D("Assets/awesomeface.png");
            tex2.PushToGPU();

            
            mat = new Material(program);
            mat.SetUniform("texture1", tex1);
            mat.SetUniform("texture2", tex2);

            cameraTransform = new TransformComponent()
            {
                Position = new Vector3(0f, 0f, -3f)
            };
            camera = new PerspectiveCameraComponent()
            {
                AspectRatio = 800/(float)600,
                NearClipping = 0.1f,
                FarClipping = 100f,
                FieldOfView = 90f,
                Transform = cameraTransform
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {
            TimeDelta = (float)_watchDelta.Elapsed.TotalSeconds;
            _watchDelta.Restart();

        }

        /// <summary>
        /// 
        /// </summary>
        public void Draw()
        {
            mat.Use();
            mat.SetTimeDelta(TimeDelta);
            mat.SetTimeTotal(TimeTotal);

            camera.Use();
            mat.SetProjectionSpace(meshTransform.Space * camera.ProjectionSpace);

            mesh.Draw();
        }
    }
}
