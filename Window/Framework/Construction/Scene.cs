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
        Texture2D tex1, tex2;

        RenderData _renderData;
        TransformData _cameraTransform;
        PerspectiveCameraData _camera;
        AmbientLightData _ambientLight;
        DirectionalLightData _directionalLight;
        MaterialData _material;
        TransformData _transform;
        VertexData _mesh;


        /// <summary>
        /// 
        /// </summary>
        public Scene()
        {
            //var inidcatorMat = new MaterialData(
            //    new ShaderProgram(
            //        new ShaderSource(ShaderType.VertexShader, "Assets/Transform.vert"),
            //        new ShaderSource(ShaderType.FragmentShader, "Assets/Transform.frag")
            //    )
            //);


            _renderData = new RenderData()
            {
                TimeBlock = new ShaderUniformBlock<TimeData>(BufferUsageHint.DynamicDraw),
                SpaceBlock = new ShaderUniformBlock<SpaceData>(BufferUsageHint.DynamicDraw)
            };


            _transform = TransformData.Default;
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

            _cameraTransform = new TransformData(new Vector3(0f, 0f, -3f));
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
            TimeSystem.Update(ref _renderData.TimeBlock.Data);
            
            _transform.Forward = new Vector3(
                MathF.Sin(_renderData.TimeBlock.Data.Total),
                -.2f,
                MathF.Cos(_renderData.TimeBlock.Data.Total)
            );
        }

        /// <summary>
        /// 
        /// </summary>
        public void Draw()
        {
            _renderData.TimeBlock.PushToGPU();

            //SpaceSystem.Update(_transform, _camera.Transform, ref _renderData.SpaceBlock.Data);
            PerspectiveCameraSystem.Use(_cameraTransform, _camera, ref _renderData);
            ShaderSystem.Use(_material.Shader, ref _renderData);
            MaterialSystem.Use(_material);
            VertexSystem.Draw(_transform, _mesh, _renderData);


            //indicator.Material.Data.Shader.Use(ref renderData);
            //indicator.Material.Use(ref renderData);
            //indicator.Material.SetProjectionSpace(meshTransform.Space * renderData.WorldToProjection);
            //indicator.Draw(ref renderData);
        }
    }
}
