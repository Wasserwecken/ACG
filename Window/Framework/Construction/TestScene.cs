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
        ShaderBlock<ShaderTime> _timeUniformBlock;
        ShaderBlockArray<DirectionalLightComponent> _directionalLightBlock;
        ShaderBlockArray<PointLightComponent> _pointLightBlock;
        ShaderBlockArray<SpotLightComponent> _spotLightBlock;
        ShaderBlock<ShaderSpaceData> _renderSpaceUniformBlock;
        UniformRegister _uniformBlockRegister;

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


            var foo = Matrix4.CreateTranslation(new Vector3(1, 2, 3));
            foo = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(45)) * foo;

            var bar = new LocalTransformComponent(new Vector3(1, 2, 3));
            bar.Forward = new Vector3(1, 1, 1);



            _sceneEntities = GLTF2System.CreateSceneEntities("./Assets/cube.glb");

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









            var a = Definitions.Buffer.Attributes["POSITION"];
            a.SetData(new float[] {
                     0.5f,  0.5f, 0.0f,  // top right
                     0.5f, -0.5f, 0.0f,  // bottom right
                    -0.5f, -0.5f, 0.0f,  // bottom left
                    -0.5f,  0.5f, 0.0f   // top left 
                });

            var b = new ArrayBufferAsset(new VertexAttributeAsset[] { a }, BufferUsageHint.StaticDraw);
            ArrayBufferSystem.Update(b);

            var i = new IndicieBufferAsset(BufferUsageHint.StaticDraw);
            i.SetData(new uint[] { 0, 1, 3, 1, 2, 3 });

            p = new VertexPrimitiveAsset(b, i);
            VertexPrimitiveSystem.PushToGPU(p);
        }

        VertexPrimitiveAsset p;

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
            foreach(var camera in cameras)
            {
                camera.TryGetComponent<WorldTransformComponent>(out var cameraTransform);
                camera.TryGetComponent<PerspectiveCameraComponent>(out var perspectiveCamera);
                PerspectiveCameraSystem.Use(cameraTransform, perspectiveCamera, ref viewSpace);

                var primitives = _sceneEntities.Where(e => e.HasComponents(typeof(WorldTransformComponent), typeof(PrimitiveRenderComponent)));
                foreach(var primitve in primitives)
                {
                    primitve.TryGetComponent<WorldTransformComponent>(out var primitiveTransform);
                    primitve.TryGetComponent<PrimitiveRenderComponent>(out var primitiveRender);

                    ShaderSystem.Use(primitiveRender.Material.Shader, _uniformBlockRegister);
                    MaterialSystem.Use(primitiveRender.Material);
                    RenderSpaceSystem.Update(primitiveTransform, viewSpace, ref _renderSpaceUniformBlock.Data);
                    _renderSpaceUniformBlock.PushToGPU();
                    VertexPrimitiveSystem.Draw(primitiveRender.Primitive);
                }
            }
        }
    }
}
