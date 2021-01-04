using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Framework
{
    public class TestScene
    {
        ShaderBlock<ShaderTime> _timeUniformBlock;
        ShaderBlockArray<DirectionalLightComponent> _directionalLightBlock;
        ShaderBlockArray<PointLightComponent> _pointLightBlock;
        ShaderBlockArray<SpotLightComponent> _spotLightBlock;
        ShaderBlock<ShaderSpaceData> _renderSpaceUniformBlock;
        UniformRegister _uniformBlockRegister;

        /// <summary>
        /// 
        /// </summary>
        public TestScene()
        {
            Console.WriteLine(GL.GetString(StringName.Version));
            Console.WriteLine(GL.GetString(StringName.ShadingLanguageVersion));
            Console.WriteLine(GL.GetString(StringName.Extensions));
            Console.WriteLine(GL.GetString(StringName.Renderer));
            Console.WriteLine(GL.GetString(StringName.Vendor));

            var scene = GLTF2System.CreateScene("./Assets/acg.glb");

            _timeUniformBlock = new ShaderBlock<ShaderTime>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
            _renderSpaceUniformBlock = new ShaderBlock<ShaderSpaceData>(BufferRangeTarget.UniformBuffer, BufferUsageHint.DynamicDraw);
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
        }

        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {
            TimeSystem.Update(ref _timeUniformBlock.Data);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Draw()
        {
            _timeUniformBlock.PushToGPU();



            //PerspectiveCameraSystem.Use(_cameraTransform, _camera, ref _viewSpace);
            //ShaderSystem.Use(_material.Shader, _uniformBlockRegister);
            //MaterialSystem.Use(_material);
            //RenderSpaceSystem.Update(_meshTransform, _viewSpace, ref _renderSpaceUniformBlock.Data);
            
            _renderSpaceUniformBlock.PushToGPU();

        }
    }
}
