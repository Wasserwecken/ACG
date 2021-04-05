using Framework.Assets.Materials;
using Framework.Assets.Shader;
using Framework.Assets.Textures;
using Framework.Assets.Verticies;
using Framework.ECS.Components.Relation;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Transform;
using Framework.ECS.GLTF2.Assets;
using Framework.ECS.GLTF2.Components;
using Framework.Extensions;
using SharpGLTF.Schema2;
using System.Collections.Generic;
using System.Linq;

namespace Framework.ECS.GLTF2
{
    public static class GLTF2Loader
    {
        private static Dictionary<Image, ImageAsset> _images;
        private static Dictionary<Texture, TextureBaseAsset> _textures;
        private static Dictionary<Material, MaterialAsset> _materials;
        private static Dictionary<Mesh, MeshAsset> _meshs;
        private static Dictionary<PunctualLight, IComponent> _lights;
        private static Dictionary<Camera, IComponent> _cameras;
        private static Dictionary<Node, Entity> _entities;
        private static ShaderProgramAsset _defaultShader;

        public static List<Entity> Load(string filePath, ShaderProgramAsset defaultShader)
        {
            var _gltfRoot = ModelRoot.Load(filePath);
            _defaultShader = defaultShader;

            _images = new Dictionary<Image, ImageAsset>();
            foreach (var gltfImage in _gltfRoot.LogicalImages)
                _images.Add(gltfImage, CreatorImageAsset.Create(gltfImage));

            _textures = new Dictionary<Texture, TextureBaseAsset>();
            foreach (var gltfTexture in _gltfRoot.LogicalTextures)
                _textures.Add(gltfTexture, CreatorTextureAsset.Create(gltfTexture, _images));

            _materials = new Dictionary<Material, MaterialAsset>();
            foreach (var gltfMaterial in _gltfRoot.LogicalMaterials)
                _materials.Add(gltfMaterial, CreatorMaterialAsset.Create(gltfMaterial, _textures));

            _meshs = new Dictionary<Mesh, MeshAsset>();
            foreach (var gltfMesh in _gltfRoot.LogicalMeshes)
                _meshs.Add(gltfMesh, CreatorMeshAsset.Create(gltfMesh));

            _lights = new Dictionary<PunctualLight, IComponent>();
            foreach (var gltfLight in _gltfRoot.LogicalPunctualLights)
                _lights.Add(gltfLight, CreatorLightComponent.Create(gltfLight));

            _cameras = new Dictionary<Camera, IComponent>();
            foreach (var gltfCamera in _gltfRoot.LogicalCameras)
                _cameras.Add(gltfCamera, CreatorCameraComponent.Create(gltfCamera));

            _entities = new Dictionary<Node, Entity>();
            foreach (var gltfNode in _gltfRoot.LogicalNodes)
                _entities.Add(gltfNode, CreateEntity(gltfNode));

            foreach (var gltfNode in _gltfRoot.LogicalNodes)
                SetRelation(gltfNode);

            return _entities.Values.ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        private static void SetRelation(Node gltfNode)
        {
            if (_entities.TryGetValue(gltfNode, out var childEntity) && gltfNode.VisualParent != null && _entities.TryGetValue(gltfNode.VisualParent, out var parentEntity))
            {
                childEntity.Components.Add(new ChildComponent() { Parent = parentEntity });

                if (parentEntity.Components.TryGet<ParentComponent>(out var childrenComponent))
                    childrenComponent.Children.Add(childEntity);
                else
                    parentEntity.Components.Add(new ParentComponent() { Children = new List<Entity>() { childEntity } });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static Entity CreateEntity(Node gltfNode)
        {
            var entity = new Entity(gltfNode.Name);
            entity.Components.Add(new TransformComponent() { LocalSpace = gltfNode.LocalMatrix.ToOpenTK() });
                
            if (gltfNode.PunctualLight != null && _lights.TryGetValue(gltfNode.PunctualLight, out var lightComponent))
                entity.Components.Add(lightComponent);

            //if (gltfNode.Camera != null && _cameras.TryGetValue(gltfNode.Camera, out var cameraComponent))
            //    entity.Components.Add(cameraComponent);

            if (gltfNode.Mesh != null && _meshs.TryGetValue(gltfNode.Mesh, out var meshAsset))
            {
                var renderer = new MeshComponent() { Mesh = meshAsset };
                renderer.Shaders.Add(_defaultShader);
                foreach (var gltfPrimitive in gltfNode.Mesh.Primitives)
                {
                    if (gltfPrimitive.Material == null)
                        renderer.Materials.Add(Default.Material.PBR);
                    else if (_materials.TryGetValue(gltfPrimitive.Material, out var materialAsset))
                        renderer.Materials.Add(materialAsset);
                }

                entity.Components.Add(renderer);
            }

            return entity;
        }
    }
}
