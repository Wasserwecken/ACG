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

        ShaderBlock<ShaderTime> _timeUniformBlock;
        ShaderBlockArray<DirectionalLightComponent> _directionalLightBlock;
        ShaderBlockArray<PointLightComponent> _pointLightBlock;
        ShaderBlockArray<SpotLightComponent> _spotLightBlock;
        ShaderBlock<RenderSpaceData> _renderSpaceUniformBlock;
        UniformRegister _uniformBlockRegister;

        ViewSpaceData _viewSpace;
        TransformComponent _cameraTransform;
        PerspectiveCameraComponent _camera;
        MaterialData _material;
        TransformComponent _meshTransform;

        /// <summary>
        /// 
        /// </summary>
        public Scene()
        {
            Console.WriteLine(GL.GetString(StringName.Version));
            Console.WriteLine(GL.GetString(StringName.ShadingLanguageVersion));
            Console.WriteLine(GL.GetString(StringName.Extensions));
            Console.WriteLine(GL.GetString(StringName.Renderer));
            Console.WriteLine(GL.GetString(StringName.Vendor));

            GLTFLoader.Load();

            _timeUniformBlock = new ShaderBlock<ShaderTime>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
            _renderSpaceUniformBlock = new ShaderBlock<RenderSpaceData>(BufferRangeTarget.UniformBuffer, BufferUsageHint.DynamicDraw);
            _directionalLightBlock = new ShaderBlockArray<DirectionalLightComponent>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
            _pointLightBlock = new ShaderBlockArray<PointLightComponent>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
            _spotLightBlock = new ShaderBlockArray<SpotLightComponent>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
            _uniformBlockRegister = new UniformRegister(new IShaderBlock[]
            {
                _timeUniformBlock,
                _renderSpaceUniformBlock,
                _directionalLightBlock,
                _pointLightBlock,
                _spotLightBlock
            });


            _meshTransform = TransformComponent.Default;
            var meshObject = Load3DHelper.Load("Assets/ape.obj")[0];




            tex1 = new Texture2D("Assets/wall.jpg");
            tex1.PushToGPU();

            tex2 = new Texture2D("Assets/awesomeface.png");
            tex2.PushToGPU();


            var shader = ShaderProgramSystem.Create( "Lit",
                ShaderSourceSystem.Create(ShaderType.VertexShader, "Assets/shader.vert"),
                ShaderSourceSystem.Create(ShaderType.FragmentShader, "Assets/shader.frag")
            );
            _material = new MaterialData(shader);
            _material.SetUniform("texture1", tex1);
            _material.SetUniform("texture2", tex2);

            _cameraTransform = new TransformComponent(new Vector3(0f, 0f, -3f));
            _camera = new PerspectiveCameraComponent()
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
            _timeUniformBlock.PushToGPU();

            PerspectiveCameraSystem.Use(_cameraTransform, _camera, ref _viewSpace);
            ShaderSystem.Use(_material.Shader, _uniformBlockRegister);
            MaterialSystem.Use(_material);

            RenderSpaceSystem.Update(_meshTransform, _viewSpace, ref _renderSpaceUniformBlock.Data);
            _renderSpaceUniformBlock.PushToGPU();

        }
    }
}
