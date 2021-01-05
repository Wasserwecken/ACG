using System;
using System.Collections.Generic;
using System.Text;
using SharpGLTF.Schema2;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Linq;

namespace Framework
{
    public static class GLTF2System
    {
        private static ShaderProgramAsset _defaultShader;
        private static MaterialAsset _defaultMaterial;
        private static ModelRoot _gltfRoot;
        private static readonly Dictionary<Material, MaterialAsset> _materials;
        private static readonly Dictionary<MeshPrimitive, PrimitiveRenderComponent> _primitiveComponents;
        private static readonly Dictionary<Camera, PerspectiveCameraComponent> _cameras;
        private static readonly Dictionary<PunctualLight, IEntityComponent> _lights;

        /// <summary>
        /// 
        /// </summary>
        static GLTF2System()
        {
            _defaultShader = new ShaderProgramAsset("DefaultLit");
            var _defaultFragment = new ShaderSourceAsset(ShaderType.FragmentShader, "./Assets/shader.frag");
            var _defaultVertex = new ShaderSourceAsset(ShaderType.VertexShader, "./Assets/shader.vert");
            _defaultMaterial = new MaterialAsset("Default", _defaultShader);

            ShaderSourceSystem.LoadAndCompile(_defaultVertex);
            ShaderSourceSystem.LoadAndCompile(_defaultFragment);
            ShaderProgramSystem.CreateAndAnalyse(_defaultShader, _defaultVertex, _defaultFragment);

            _materials = new Dictionary<Material, MaterialAsset>();
            _primitiveComponents = new Dictionary<MeshPrimitive, PrimitiveRenderComponent>();
            _cameras = new Dictionary<Camera, PerspectiveCameraComponent>();
            _lights = new Dictionary<PunctualLight, IEntityComponent>();
        }

        /// <summary>
        /// 
        /// </summary>
        public static List<Entity> CreateSceneEntities(string filePath)
        {
            _gltfRoot = ModelRoot.Load(filePath);
            GenerateMaterials();

            GenerateRenderComponents();
            GenerateCameras();
            GenerateLights();

            return GenerateSceneEntity(_gltfRoot.DefaultScene);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void GenerateMaterials()
        {
            _materials.Clear();
            foreach (var gltfMaterial in _gltfRoot.LogicalMaterials)
            {
                _materials.Add(gltfMaterial, new MaterialAsset(gltfMaterial.Name, _defaultShader));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static void GenerateRenderComponents()
        {
            _primitiveComponents.Clear();

            foreach (var glTFmesh in _gltfRoot.LogicalMeshes)
            {
                var primitives = new List<VertexPrimitiveAsset>();

                foreach (var glTFprimitive in glTFmesh.Primitives)
                {
                    var vertexAttributes = new List<VertexAttributeAsset>();
                    foreach (var glTFaccessor in glTFprimitive.VertexAccessors)
                    {
                        if (!Definitions.Buffer.Attributes.TryGetValue(glTFaccessor.Key, out var attributeAsset))
                            attributeAsset = new VertexAttributeAsset(
                                glTFaccessor.Key,
                                glTFaccessor.Value.LogicalIndex,
                                glTFaccessor.Value.Format.ByteSize,
                                glTFaccessor.Value.Format.Normalized,
                                (VertexAttribPointerType)glTFaccessor.Value.Format.Encoding
                            );

                        attributeAsset.SetData(glTFaccessor.Value.SourceBufferView.Content.ToArray());
                        vertexAttributes.Add(attributeAsset);
                    }

                    var arrayBuffer = new ArrayBufferAsset(vertexAttributes.ToArray(), BufferUsageHint.StaticDraw);
                    ArrayBufferSystem.Update(arrayBuffer);

                    var indicieBuffer = new IndicieBufferAsset(BufferUsageHint.StaticDraw);
                    indicieBuffer.SetData(glTFprimitive.GetIndices().ToArray());

                    var primitive = new VertexPrimitiveAsset(arrayBuffer, indicieBuffer) { Mode = PolygonMode.Fill, Type = (OpenTK.Graphics.OpenGL.PrimitiveType)glTFprimitive.DrawPrimitiveType };
                    VertexPrimitiveSystem.PushToGPU(primitive);

                    _primitiveComponents.Add(glTFprimitive, new PrimitiveRenderComponent()
                    {
                        Primitive = primitive,
                        Material = glTFprimitive.Material != null ? _materials[glTFprimitive.Material] : _defaultMaterial
                    });
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static void GenerateCameras()
        {
            _cameras.Clear();

            foreach (var glTFcamera in _gltfRoot.LogicalCameras)
            {
                if (glTFcamera.Settings.IsPerspective)
                {
                    var glTFperspective = (CameraPerspective)glTFcamera.Settings;
                    _cameras.Add(glTFcamera, new PerspectiveCameraComponent()
                    {
                        NearClipping = glTFperspective.ZNear,
                        FarClipping = glTFperspective.ZFar,
                        FieldOfView = glTFperspective.VerticalFOV,
                        ClearMask = ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit,
                        ClearColor = new Vector4(0.3f),
                        AspectRatio = 1f
                    });
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static void GenerateLights()
        {
            _lights.Clear();

            foreach (var glTFlight in _gltfRoot.LogicalPunctualLights)
            {
                switch (glTFlight.LightType)
                {
                    case PunctualLightType.Directional:
                        _lights.Add(glTFlight, new DirectionalLightComponent()
                        {
                            Color = new Vector3(glTFlight.Color.X, glTFlight.Color.Y, glTFlight.Color.Z) * glTFlight.Intensity,
                            AmbientFactor = 0.02f,
                        });
                        break;

                    case PunctualLightType.Point:
                        _lights.Add(glTFlight, new PointLightComponent()
                        {
                            Color = new Vector3(glTFlight.Color.X, glTFlight.Color.Y, glTFlight.Color.Z) * glTFlight.Intensity,
                            AmbientFactor = 0.02f,
                        });
                        break;

                    case PunctualLightType.Spot:
                        _lights.Add(glTFlight, new SpotLightComponent()
                        {
                            Color = new Vector3(glTFlight.Color.X, glTFlight.Color.Y, glTFlight.Color.Z) * glTFlight.Intensity,
                            AmbientFactor = 0.02f,
                            OuterAngle = glTFlight.OuterConeAngle,
                            InnerAngle = glTFlight.InnerConeAngle,
                        });
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static List<Entity> GenerateSceneEntity(Scene scene)
        {
            var entities = new List<Entity>();
            var sceneEntity = new Entity("scene");

            entities.Add(sceneEntity);
            foreach(var gltfNode in scene.VisualChildren)
            {
                var entitiy = GenerateEntity(gltfNode, entities);
                entitiy.AddComponent(new ParentEntityComponent() { Parent = sceneEntity });
                sceneEntity.AddComponent(new ChildEntityComponent() { Child = entitiy });
            }

            return entities;
        }

        /// <summary>
        /// 
        /// </summary>
        private static Entity GenerateEntity(Node gltfEntity, List<Entity> entities)
        {
            var newEntity = new Entity(gltfEntity.Name);
            entities.Add(newEntity);

            newEntity.AddComponent(new WorldTransformComponent() { Space = gltfEntity.WorldMatrix.ToOpenTK() });

            if (gltfEntity.Camera != null && _cameras.TryGetValue(gltfEntity.Camera, out var camera))
                newEntity.AddComponent(camera);

            if (gltfEntity.Mesh != null)
                foreach (var gltfPrimitive in gltfEntity.Mesh.Primitives)
                    if (_primitiveComponents.TryGetValue(gltfPrimitive, out var renderComponent))
                        newEntity.AddComponent(renderComponent);

            if (gltfEntity.PunctualLight != null && _lights.TryGetValue(gltfEntity.PunctualLight, out var lightComponent))
                newEntity.AddComponent(lightComponent);

            foreach (var gltfChild in gltfEntity.VisualChildren)
            {
                var childEntity = GenerateEntity(gltfChild, entities);
                childEntity.AddComponent(new ParentEntityComponent() { Parent = newEntity });
                newEntity.AddComponent(new ChildEntityComponent() { Child = childEntity });
            }

            return newEntity;
        }
    }
}
