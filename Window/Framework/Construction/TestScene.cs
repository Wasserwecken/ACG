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
        ShaderBlock<ShaderSpace> _renderSpaceUniformBlock;
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
            _renderSpaceUniformBlock = new ShaderBlock<ShaderSpace>(BufferRangeTarget.UniformBuffer, BufferUsageHint.DynamicDraw);
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


            var s = new ShaderProgramAsset("DefaultLit");
            var f = new ShaderSourceAsset(ShaderType.FragmentShader, "./Assets/shader.frag");
            var v = new ShaderSourceAsset(ShaderType.VertexShader, "./Assets/shader.vert");
            ShaderSourceSystem.LoadAndCompile(f);
            ShaderSourceSystem.LoadAndCompile(v);
            ShaderProgramSystem.CreateAndAnalyse(s, new ShaderSourceAsset[] { f, v });
            var m = new MaterialAsset("Default", s);




            var a = Definitions.Buffer.Attributes["POSITION"];
            var b = new ArrayBufferAsset(new VertexAttributeAsset[] { a }, BufferUsageHint.StaticDraw);
            var i = new IndicieBufferAsset(BufferUsageHint.StaticDraw);
            a.SetData(new float[] { 0.5f,  0.5f, 0.0f, 0.5f, -0.5f, 0.0f, -0.5f, -0.5f, 0.0f, -0.5f,  0.5f, 0.0f });
            i.SetData(new uint[] { 0, 1, 3, 1, 2, 3 });
            var p = new VertexPrimitiveAsset(b, i);
            VertexPrimitiveSystem.PushToGPU(p);




            var entP = new Entity("p");
            entP.AddComponent(new WorldTransformComponent(Matrix4.CreateTranslation(0f, 0f, -2f)));
            entP.AddComponent(new PrimitiveRenderComponent(p, m));

            var entC = new Entity("c");
            entC.AddComponent(new WorldTransformComponent(Matrix4.CreateTranslation(0f, 0f, -1f)));
            entC.AddComponent(new PerspectiveCameraComponent()
            {
                AspectRatio = 1f,
                ClearColor = new Vector4(0.4f, 0.4f, 0.4f, 1f),
                ClearMask = ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit,
                FieldOfView = MathHelper.DegreesToRadians(90f),
                NearClipping = 0.01f,
                FarClipping = 100f
            });




            //_sceneEntities = new List<Entity>();
            //_sceneEntities.Add(entP);
            //_sceneEntities.Add(entC);
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
                PerspectiveCameraSystem.Use(cameraTransform, perspectiveCamera, ref viewSpace);

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
