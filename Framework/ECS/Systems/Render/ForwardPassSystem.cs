using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Materials;
using Framework.Assets.Shader;
using Framework.Assets.Shader.Block;
using Framework.Assets.Shader.Block.Data;
using Framework.Assets.Textures;
using Framework.Assets.Verticies;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Scene;
using Framework.ECS.Components.Transform;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Linq;

namespace Framework.ECS.Systems.Render
{
    [With(typeof(TransformComponent))]
    [With(typeof(PerspectiveCameraComponent))]
    public class ForwardPassSystem : AEntitySetSystem<bool>
    {
        private readonly Entity _worldComponents;
        private EntitySet _graphSet;

        private readonly ShaderBlockSingle<ShaderViewSpace> _viewSpaceBlock;
        private readonly ShaderBlockSingle<ShaderPrimitiveSpace> _primitiveSpaceBlock;

        private readonly List<MaterialAsset> _materials;
        private readonly List<TextureBaseAsset> _textures;
        private readonly List<ShaderProgramAsset> _shaders;
        private readonly List<TransformComponent> _transforms;
        private readonly List<VertexPrimitiveAsset> _primitives;

        private readonly Dictionary<ShaderProgramAsset,
           Dictionary<MaterialAsset,
               Dictionary<TransformComponent,
                   List<VertexPrimitiveAsset>>>> _graph;

        /// <summary>
        /// 
        /// </summary>
        public ForwardPassSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
            _graphSet = World.GetEntities().With<TransformComponent>().With<PrimitiveComponent>().AsSet();

            _viewSpaceBlock = new ShaderBlockSingle<ShaderViewSpace>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
            _primitiveSpaceBlock = new ShaderBlockSingle<ShaderPrimitiveSpace>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);

            _materials = new List<MaterialAsset>();
            _textures = new List<TextureBaseAsset>();
            _shaders = new List<ShaderProgramAsset>();
            _transforms = new List<TransformComponent>();
            _primitives = new List<VertexPrimitiveAsset>();
            _graph = new Dictionary<ShaderProgramAsset, Dictionary<MaterialAsset, Dictionary<TransformComponent, List<VertexPrimitiveAsset>>>>();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void PreUpdate(bool state)
        {
            _graph.Clear();
            _shaders.Clear();
            _materials.Clear();
            _transforms.Clear();
            _primitives.Clear();

            foreach (var entity in _graphSet.GetEntities())
            {
                var primitive = entity.Get<PrimitiveComponent>();
                var transform = entity.Get<TransformComponent>();

                var shader = primitive.Shader;
                var material = primitive.Material;
                var verticies = primitive.Primitive;

                if (!_graph.ContainsKey(shader))
                {
                    _graph.Add(shader, new Dictionary<MaterialAsset, Dictionary<TransformComponent, List<VertexPrimitiveAsset>>>());
                    _shaders.Add(shader);
                }

                if (!_graph[shader].ContainsKey(material))
                {
                    _graph[shader].Add(material, new Dictionary<TransformComponent, List<VertexPrimitiveAsset>>());
                    _materials.Add(material);
                }

                if (!_graph[shader][material].ContainsKey(transform))
                {
                    _graph[shader][material].Add(transform, new List<VertexPrimitiveAsset>());
                    _transforms.Add(transform);
                }

                _graph[shader][material][transform].Add(verticies);
                _primitives.Add(verticies);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, in Entity entity)
        {
            var cameraData = entity.Get<PerspectiveCameraComponent>();
            var cameraTransform = entity.Get<TransformComponent>();
            var aspectRatio = _worldComponents.Get<AspectRatioComponent>();


            TextureBaseAsset skyboxTexture = Defaults.Texture.SkyboxCoast;
            var projectionSpace = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(cameraData.FieldOfView),
                aspectRatio.Ratio,
                cameraData.NearClipping,
                cameraData.FarClipping
            );

            GL.Viewport(0, 0, aspectRatio.Width, aspectRatio.Height);
            UseCamera(cameraData);
            _viewSpaceBlock.Data = CreateViewSpace(cameraTransform, projectionSpace);
            _viewSpaceBlock.PushToGPU();

            foreach (var shaderRelation in _graph)
            {
                UseShader(shaderRelation.Key);

                foreach (var materialRelation in shaderRelation.Value)
                {
                    materialRelation.Key.SetUniform(Definitions.Shader.Uniform.ReflectionMap, skyboxTexture);
                    UseMaterial(materialRelation.Key);
                    SetUniforms(materialRelation.Key, shaderRelation.Key);

                    foreach (var transformRelation in materialRelation.Value)
                    {
                        _primitiveSpaceBlock.Data = CreatePrimitiveSpace(transformRelation.Key, cameraTransform, projectionSpace);
                        _primitiveSpaceBlock.PushToGPU();

                        foreach (var primitive in transformRelation.Value)
                            Draw(primitive);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void UseCamera(PerspectiveCameraComponent camera)
        {
            GL.ClearColor(camera.ClearColor.X, camera.ClearColor.Y, camera.ClearColor.Z, camera.ClearColor.W);
            GL.Clear(camera.ClearMask);
        }

        /// <summary>
        /// 
        /// </summary>
        private void UseShader(ShaderProgramAsset shader)
        {
            GL.UseProgram(shader.Handle);
            foreach (var block in ShaderBlockRegister.Blocks)
                if (shader.IdentifierToLayout.TryGetValue(block.Name, out var blockLayout))
                    GL.BindBufferBase(block.Target, blockLayout, block.Handle);
        }

        /// <summary>
        /// 
        /// </summary>
        private void UseMaterial(MaterialAsset material)
        {
            GL.ShadeModel(material.Model);
            GL.FrontFace(material.FaceDirection);

            if (material.IsDepthTesting)
            {
                GL.Enable(EnableCap.DepthTest);
                GL.DepthFunc(material.DepthTest);
            }
            else
                GL.Disable(EnableCap.DepthTest);

            if (material.IsCulling)
            {
                GL.Enable(EnableCap.CullFace);
                GL.CullFace(material.CullingMode);
            }
            else
                GL.Disable(EnableCap.CullFace);

            if (material.IsTransparent)
            {
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(material.SourceBlend, material.DestinationBlend);
            }
            else
                GL.Disable(EnableCap.Blend);
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetUniforms(MaterialAsset material, ShaderProgramAsset shader)
        {
            foreach (var uniform in material.UniformFloats)
                if (shader.IdentifierToLayout.TryGetValue(uniform.Key, out var layout))
                    GL.Uniform1(layout, uniform.Value);

            foreach (var uniform in material.UniformVecs)
                if (shader.IdentifierToLayout.TryGetValue(uniform.Key, out var layout))
                    GL.Uniform4(layout, uniform.Value);

            foreach (var uniform in material.UniformMats)
                if (shader.IdentifierToLayout.TryGetValue(uniform.Key, out var layout))
                {
                    var foo = uniform.Value;
                    GL.UniformMatrix4(layout, false, ref foo);
                }

            foreach (var uniform in material.UniformTextures)
                if (shader.IdentifierToLayout.TryGetValue(uniform.Key, out var layout))
                {
                    GL.Uniform1(layout, layout);
                    GL.ActiveTexture(TextureUnit.Texture0 + layout);
                    GL.BindTexture(uniform.Value.Target, uniform.Value.Handle);
                }

            foreach (var uniformTexture in shader.Uniforms.Where
                (f => f.Type == ActiveUniformType.Sampler2D && !material.UniformTextures.ContainsKey(f.Name)))
            {
                GL.Uniform1(uniformTexture.Layout, uniformTexture.Layout);
                GL.ActiveTexture(TextureUnit.Texture0 + uniformTexture.Layout);

                if (uniformTexture.Name.ToLower().Contains("normal"))
                    GL.BindTexture(Defaults.Texture.Normal.Target, Defaults.Texture.Normal.Handle);
                else
                    GL.BindTexture(Defaults.Texture.White.Target, Defaults.Texture.White.Handle);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private ShaderViewSpace CreateViewSpace(TransformComponent transform, Matrix4 projection)
        {
            return new ShaderViewSpace
            {
                WorldToView = transform.WorldSpaceInverse,
                WorldToProjection = transform.WorldSpaceInverse * projection,

                WorldToViewRotation = transform.WorldSpaceInverse.ClearScale().ClearTranslation(),
                WorldToProjectionRotation = transform.WorldSpaceInverse.ClearScale().ClearTranslation() * projection,

                ViewPosition = new Vector4(transform.Position, 1),
                ViewDirection = new Vector4(-transform.Forward, 0)
            };
        }

        /// <summary>
        /// 
        /// </summary>
        private ShaderPrimitiveSpace CreatePrimitiveSpace(TransformComponent primitiveTransform, TransformComponent viewTransform, Matrix4 projection)
        {
            return new ShaderPrimitiveSpace
            {
                LocalToWorld = primitiveTransform.WorldSpace,
                LocalToView = primitiveTransform.WorldSpace * viewTransform.WorldSpaceInverse,
                LocalToProjection = primitiveTransform.WorldSpace * viewTransform.WorldSpaceInverse * projection,

                LocalToWorldRotation = primitiveTransform.WorldSpace.ClearScale(),
                LocalToViewRotation = (primitiveTransform.WorldSpace * viewTransform.WorldSpaceInverse).ClearScale().ClearTranslation(),
                LocalToProjectionRotation = (primitiveTransform.WorldSpace * viewTransform.WorldSpaceInverse).ClearScale().ClearTranslation() * projection,
            };
        }

        /// <summary>
        /// 
        /// </summary>
        private void Draw(VertexPrimitiveAsset primitive)
        {
            GL.BindVertexArray(primitive.Handle);
            GL.PolygonMode(MaterialFace.FrontAndBack, primitive.Mode);

            if (primitive.IndicieBuffer != null)
                GL.DrawElements(primitive.Type, primitive.IndicieBuffer.Indicies.Length, DrawElementsType.UnsignedInt, 0);
            else
                GL.DrawArrays(primitive.Type, 0, primitive.ArrayBuffer.ElementCount);
        }
    }
}
