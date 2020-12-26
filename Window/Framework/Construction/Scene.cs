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
        Material mat;
        MeshObject mesh;
        TransformData meshTransform;
        TransformIndicator indicator;

        DirectionalLight dirLight = new DirectionalLight();


        PerspectiveCamera camera;



        /// <summary>
        /// 
        /// </summary>
        public Scene()
        {
            _watchTotal = new Stopwatch();
            _watchTotal.Start();

            _watchDelta = new Stopwatch();

            var inidcatorMat = new MaterialData(
                new ShaderProgram(
                    new ShaderSource(ShaderType.VertexShader, "Assets/Transform.vert"),
                    new ShaderSource(ShaderType.FragmentShader, "Assets/Transform.frag")
                )
            );

            indicator = new TransformIndicator()
            {
                Transform = new TransformData(Vector3.Zero),
                IndicatorObject = new TransformIndicatorObject(),
                Material = new Material() { Data = inidcatorMat }
            };

            mesh = Load3DHelper.Load("Assets/ape.obj")[0];
            mesh.PushToGPU();

            tex1 = new Texture2D("Assets/wall.jpg");
            tex1.PushToGPU();

            tex2 = new Texture2D("Assets/awesomeface.png");
            tex2.PushToGPU();


            meshTransform = TransformData.Default;

            var matData = new MaterialData(
                new ShaderProgram(
                    new ShaderSource(ShaderType.VertexShader, "Assets/shader.vert"),
                    new ShaderSource(ShaderType.FragmentShader, "Assets/shader.frag")
            ));
            mat = new Material() { Data = matData };
            mat.SetUniform("texture1", tex1);
            mat.SetUniform("texture2", tex2);


            camera = new PerspectiveCamera()
            {
                BaseData = new CameraData()
                {
                    ClearColor = new Vector4(0.3f),
                    ClearMask = ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit,
                    Transform = new TransformData(new Vector3(0f, 0f, -3f)),
                    AspectRatio = 800 / (float)600,
                    NearClipping = 0.1f,
                    FarClipping = 100f,
                },
                PerspectiveData = new PerspectiveCameraData()
                {
                    FieldOfView = 60f,
                }
            };

            dirLight = new DirectionalLight()
            {
                Transform = TransformData.Default,
                Color = new Vector4(1f)
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

            camera.Use(ref renderData);
            renderData.ViewPosition = camera.BaseData.Transform.Position;

            mat.Data.Shader.Use(ref renderData);
            mat.Use(ref renderData);

            renderData.LocalToWorld = meshTransform.Space;
            renderData.LocalToProjection = renderData.LocalToWorld * renderData.WorldToProjection;

            mat.SetWorldSpace(renderData.LocalToWorld);
            mat.SetProjectionSpace(renderData.LocalToProjection);
            mat.SetViewPosition(renderData.ViewPosition);
            mat.SetProjectionSpace(renderData.LocalToProjection);
            mesh.Draw(ref renderData);

            indicator.Material.Data.Shader.Use(ref renderData);
            indicator.Material.Use(ref renderData);
            indicator.Material.SetProjectionSpace(meshTransform.Space * renderData.WorldToProjection);
            indicator.Draw(ref renderData);
        }
    }
}
