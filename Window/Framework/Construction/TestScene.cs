using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Framework
{
    public class TestScene
    {
        public AspectRatioComponent _aspectRatio;

        ShaderBlock<ShaderTime> _timeUniformBlock;
        ShaderBlock<ShaderSpace> _renderSpaceUniformBlock;
        ShaderBlockArray<ShaderDirectionalLight> _directionalLightBlock;
        ShaderBlockArray<ShaderPointLight> _pointLightBlock;
        ShaderBlockArray<ShaderSpotLight> _spotLightBlock;

        List<Entity> _sceneEntities;

        /// <summary>
        /// 
        /// </summary>
        public TestScene()
        {
            Console.WriteLine(GL.GetString(StringName.Version));
            Console.WriteLine(GL.GetString(StringName.ShadingLanguageVersion));
            Console.WriteLine(GL.GetString(StringName.Renderer));
            Console.WriteLine(GL.GetError());

            var shader = Defaults.Shader.Program.MeshBlinnPhong;
            var material = Defaults.Material.BlinnPhong;
            var primitive = Defaults.Vertex.Primitive.Sphere;




            //_sceneEntities = GLTF2System.CreateSceneEntities("./Assets/acg.glb");

            _timeUniformBlock = new ShaderBlock<ShaderTime>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
            _renderSpaceUniformBlock = new ShaderBlock<ShaderSpace>(BufferRangeTarget.UniformBuffer, BufferUsageHint.DynamicDraw);
            _directionalLightBlock = new ShaderBlockArray<ShaderDirectionalLight>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
            _pointLightBlock = new ShaderBlockArray<ShaderPointLight>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
            _spotLightBlock = new ShaderBlockArray<ShaderSpotLight>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);

            var directionalLights = _sceneEntities.Where(e => e.HasComponents(typeof(WorldTransformComponent), typeof(DirectionalLightComponent))).ToArray();
            var pointLights = _sceneEntities.Where(e => e.HasComponents(typeof(WorldTransformComponent), typeof(PointLightComponent))).ToArray();
            var spotLights = _sceneEntities.Where(e => e.HasComponents(typeof(WorldTransformComponent), typeof(SpotLightComponent))).ToArray();

            _directionalLightBlock.Data = new ShaderDirectionalLight[directionalLights.Length];
            _pointLightBlock.Data = new ShaderPointLight[pointLights.Length];
            _spotLightBlock.Data = new ShaderSpotLight[spotLights.Length];

            ShaderLightSystem.Copy(directionalLights, ref _directionalLightBlock.Data);
            ShaderLightSystem.Copy(pointLights, ref _pointLightBlock.Data);
            ShaderLightSystem.Copy(spotLights, ref _spotLightBlock.Data);

            _directionalLightBlock.PushToGPU();
            _pointLightBlock.PushToGPU();
            _spotLightBlock.PushToGPU();
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

            var viewSpace = new ViewSpaceData();
            var cameras = _sceneEntities.Where(e => e.HasComponents(typeof(WorldTransformComponent), typeof(PerspectiveCameraComponent)));
            var primitives = _sceneEntities.Where(e => e.HasComponents(typeof(WorldTransformComponent), typeof(PrimitiveRenderComponent)));

            foreach (var camera in cameras)
            {
                camera.TryGetComponent<WorldTransformComponent>(out var cameraTransform);
                camera.TryGetComponent<PerspectiveCameraComponent>(out var perspectiveCamera);
                PerspectiveCameraSystem.Use(cameraTransform, perspectiveCamera, _aspectRatio, ref viewSpace);

                foreach(var primitive in primitives)
                {
                    primitive.TryGetComponent<WorldTransformComponent>(out var primitiveTransform);
                    foreach(var primitiveRender in primitive.GetComponents<PrimitiveRenderComponent>())
                    {
                        //ShaderSystem.Use(primitiveRender.Material.Shader, _shaderBlockRegister);
                        MaterialSystem.Use(primitiveRender.Material);
                        RenderSpaceSystem.Update(primitiveTransform, viewSpace, ref _renderSpaceUniformBlock.Data);
                        _renderSpaceUniformBlock.PushToGPU();
                        VertexPrimitiveManager.Draw(primitiveRender.Primitive);
                    }
                }
            }
        }
    }
}
