﻿using DefaultEcs;
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
using OpenTK.Mathematics;
using SharpGLTF.Schema2;
using System.Collections.Generic;
using System.Linq;

namespace Framework.ECS.GLTF2
{
    public static class GLTF2Loader
    {
        private static World _world;
        private static Dictionary<Image, ImageAsset> _images;
        private static Dictionary<Texture, TextureBaseAsset> _textures;
        private static Dictionary<Material, MaterialAsset> _materials;
        private static Dictionary<Mesh, MeshAsset> _meshs;
        private static Dictionary<PunctualLight, object> _lights;
        private static Dictionary<Camera, object> _cameras;
        private static Dictionary<Node, Entity> _entities;
        private static ShaderProgramAsset _defaultShader;

        public static void Load(World world, string filePath, ShaderProgramAsset defaultShader)
        {
            var _gltfRoot = ModelRoot.Load(filePath);
            _defaultShader = defaultShader;
            _world = world;

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

            _lights = new Dictionary<PunctualLight, object>();
            foreach (var gltfLight in _gltfRoot.LogicalPunctualLights)
                _lights.Add(gltfLight, CreatorLightComponent.Create(gltfLight));

            _cameras = new Dictionary<Camera, object>();
            foreach (var gltfCamera in _gltfRoot.LogicalCameras)
                _cameras.Add(gltfCamera, CreatorCameraComponent.Create(gltfCamera));

            _entities = new Dictionary<Node, Entity>();
            foreach (var gltfNode in _gltfRoot.LogicalNodes)
                _entities.Add(gltfNode, CreateEntity(gltfNode));

            foreach (var gltfNode in _gltfRoot.LogicalNodes)
                SetRelation(gltfNode);
        }

        /// <summary>
        /// 
        /// </summary>
        private static Entity CreateEntity(Node gltfNode)
        {
            var entity = _world.CreateEntity();
            entity.Set(new TransformComponent(gltfNode.LocalMatrix.ToOpenTK()));
                
            if (gltfNode.PunctualLight != null && _lights.TryGetValue(gltfNode.PunctualLight, out var lightComponent))
                entity.Set(lightComponent);

            if (gltfNode.Camera != null && _cameras.TryGetValue(gltfNode.Camera, out var cameraComponent))
                entity.Set(cameraComponent);

            if (gltfNode.Mesh != null && _meshs.TryGetValue(gltfNode.Mesh, out var meshAsset))
            {
                var renderer = MeshComponent.Default;
                renderer.Mesh = meshAsset;
                renderer.Shaders.Add(_defaultShader);

                foreach (var gltfPrimitive in gltfNode.Mesh.Primitives)
                {
                    if (gltfPrimitive.Material == null)
                        renderer.Materials.Add(Defaults.Material.PBR);
                    else if (_materials.TryGetValue(gltfPrimitive.Material, out var materialAsset))
                        renderer.Materials.Add(materialAsset);
                }

                entity.Set(renderer);
            }

            return entity;
        }

        /// <summary>
        /// 
        /// </summary>
        private static void SetRelation(Node gltfNode)
        {
            if (_entities.TryGetValue(gltfNode, out var childEntity) && gltfNode.VisualParent != null && _entities.TryGetValue(gltfNode.VisualParent, out var parentEntity))
            {
                childEntity.Set(new ChildComponent() { Parent = parentEntity });

                if (parentEntity.Has<ParentComponent>())
                    parentEntity.Get<ParentComponent>().Children.Add(childEntity);
                else
                    parentEntity.Set(new ParentComponent() { Children = new List<Entity>() { childEntity } });
            }
        }
    }
}
