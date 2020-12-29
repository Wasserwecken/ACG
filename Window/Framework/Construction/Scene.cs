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


        GlobalUniformData _globalUniform;
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


            _globalUniform = new GlobalUniformData()
            {
                TimeBlock = new ShaderUniformBlock<TimeData>(BufferUsageHint.DynamicDraw),
                SpaceBlock = new ShaderUniformBlock<SpaceData>(BufferUsageHint.DynamicDraw)
            };
            _globalUniform.TimeBlock.PushToGPU();


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
            TimeSystem.Update(ref _globalUniform.TimeBlock.Data);
            _globalUniform.TimeBlock.PushToGPU();
            
            _transform.Forward = new Vector3(
                MathF.Sin(_globalUniform.TimeBlock.Data.Total),
                -.2f,
                MathF.Cos(_globalUniform.TimeBlock.Data.Total)
            );
        }

        /// <summary>
        /// 
        /// </summary>
        public void Draw()
        {
            var renderData = new RenderData();
            var lightData = new LightData();

            lightData.SetAmbient(_ambientLight);
            lightData.SetDirectional(_directionalLight);

            SpaceSystem.Update(_transform, _camera.Transform, ref _globalUniform.SpaceBlock.Data);
            PerspectiveCameraSystem.Use(_camera, ref _globalUniform.SpaceBlock.Data);
            _globalUniform.SpaceBlock.PushToGPU();

            ShaderSystem.Use(_material.Shader, ref _globalUniform, ref renderData);
            LightSystem.Use(lightData, _material);

            MaterialSystem.Use(_material);
            VertexSystem.Draw(_transform, _mesh, _globalUniform, renderData);


            //indicator.Material.Data.Shader.Use(ref renderData);
            //indicator.Material.Use(ref renderData);
            //indicator.Material.SetProjectionSpace(meshTransform.Space * renderData.WorldToProjection);
            //indicator.Draw(ref renderData);
        }
    }
}
