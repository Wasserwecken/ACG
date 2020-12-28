using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Framework
{
    public class Scene
    {
        private Stopwatch _watchTotal;
        private Stopwatch _watchDelta;

        private float _timeDelta { get; set; }
        private float _timeTotal => (float)_watchTotal.Elapsed.TotalSeconds;

        Texture2D tex1, tex2;


        PerspectiveCameraData _camera;

        AmbientLightData _ambientLight;
        DirectionalLightData _directionalLight;

        MaterialData _material;

        TransformData meshTransform;
        VertexData _mesh;



        /// <summary>
        /// 
        /// </summary>
        public Scene()
        {
            _watchTotal = new Stopwatch();
            _watchTotal.Start();

            _watchDelta = new Stopwatch();

            //var inidcatorMat = new MaterialData(
            //    new ShaderProgram(
            //        new ShaderSource(ShaderType.VertexShader, "Assets/Transform.vert"),
            //        new ShaderSource(ShaderType.FragmentShader, "Assets/Transform.frag")
            //    )
            //);



            meshTransform = TransformData.Default;
            var meshObject = Load3DHelper.Load("Assets/ape.obj")[0];
            meshObject.PushToGPU();
            _mesh = new VertexData(meshObject);




            tex1 = new Texture2D("Assets/wall.jpg");
            tex1.PushToGPU();

            tex2 = new Texture2D("Assets/awesomeface.png");
            tex2.PushToGPU();


            var shader = ShaderProgramFactory.Create(
                new ShaderSource(ShaderType.VertexShader, "Assets/shader.vert"),
                new ShaderSource(ShaderType.FragmentShader, "Assets/shader.frag")
            );
            _material = new MaterialData(shader);
            _material.SetUniform("texture1", tex1);
            _material.SetUniform("texture2", tex2);


            _ambientLight = AmbientLightData.Default;
            _directionalLight = DirectionalLightData.Default;


            _camera = new PerspectiveCameraData()
            {
                FieldOfView = 60f,
                ClearColor = new Vector4(0.3f),
                ClearMask = ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit,
                Transform = new TransformData(new Vector3(0f, 0f, -3f)),
                AspectRatio = 800 / (float)600,
                NearClipping = 0.1f,
                FarClipping = 100f,
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {
            _timeDelta = (float)_watchDelta.Elapsed.TotalSeconds;
            _watchDelta.Restart();


            //meshTransform.Forward = new Vector3(1f, 0f, 1f);
            meshTransform.Forward = new Vector3(MathF.Sin(_timeTotal), -.2f, MathF.Cos(_timeTotal));
            //meshTransform = TransformData.Default;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Draw()
        {
            var renderData = new RenderData()
            {
                TimeDelta = _timeDelta,
                TimeTotal = _timeTotal
            };

            var lightData = new LightData();
            lightData.SetAmbient(_ambientLight);
            lightData.SetDirectional(_directionalLight);

            PerspectiveCameraSystem.Use(_camera, ref renderData);

            ShaderSystem.Use(_material.Shader, ref renderData);
            LightSystem.Use(lightData, _material);

            MaterialSystem.Use(_material);
            VertexSystem.Draw(meshTransform, _mesh, renderData);


            //indicator.Material.Data.Shader.Use(ref renderData);
            //indicator.Material.Use(ref renderData);
            //indicator.Material.SetProjectionSpace(meshTransform.Space * renderData.WorldToProjection);
            //indicator.Draw(ref renderData);
        }
    }
}
