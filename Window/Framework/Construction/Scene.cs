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

        UniformBlock<TimeData> _timeUniformBlock;
        UniformBlock<RenderSpaceData> _renderSpaceUniformBlock;
        UniformRegister _uniformBlockRegister;

        TransformData _cameraTransform;
        PerspectiveCameraData _camera;
        AmbientLightData _ambientLight;
        DirectionalLightData _directionalLight;
        MaterialData _material;
        TransformData _meshTransform;
        VertexData _mesh;


        /// <summary>
        /// 
        /// </summary>
        public Scene()
        {

            _timeUniformBlock = new UniformBlock<TimeData>("TimeBlock", BufferUsageHint.DynamicDraw);
            _renderSpaceUniformBlock = new UniformBlock<RenderSpaceData>("SpaceBlock", BufferUsageHint.DynamicDraw);
            _uniformBlockRegister = new UniformRegister(
                new IUniformBlock[] { _timeUniformBlock },
                new IUniformBlock[] { _renderSpaceUniformBlock}
            );


            _meshTransform = TransformData.Default;
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
            TimeSystem.Update(ref _timeUniformBlock.Data);
            
            _meshTransform.Forward = new Vector3(
                MathF.Sin(_timeUniformBlock.Data.Total),
                -.2f,
                MathF.Cos(_timeUniformBlock.Data.Total)
            );
        }

        /// <summary>
        /// 
        /// </summary>
        public void Draw()
        {
            var viewSpace = new ViewSpaceData();
            _timeUniformBlock.PushToGPU();

            PerspectiveCameraSystem.Use(_cameraTransform, _camera, ref viewSpace);
            ShaderSystem.Use(_material.Shader, _uniformBlockRegister);
            MaterialSystem.Use(_material);

            RenderSpaceSystem.Update(_meshTransform, viewSpace, ref _renderSpaceUniformBlock.Data);
            _renderSpaceUniformBlock.PushToGPU();

            VertexSystem.Draw(_meshTransform, _mesh, _uniformBlockRegister);
        }
    }
}
