using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Materials;
using Framework.Assets.Shader.Block;
using Framework.ECS.Components.Light;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Scene;
using Framework.ECS.Components.Transform;
using Framework.ECS.Systems.Render.OpenGL;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;

namespace Framework.ECS.Systems.RenderPipeline
{
    [With(typeof(TransformComponent))]
    [With(typeof(ReflectionProbeComponent))]
    public class ReflectionDeferredPassSystem : AEntitySetSystem<bool>
    {
        private readonly Entity _worldComponents;
        private readonly EntitySet _renderCandidates;
        private readonly ShaderViewSpaceBlock _viewBlock;
        private readonly ShaderDeferredViewBlock _viewDeferredBlock;
        private readonly MaterialAsset _lightMaterial;

        /// <summary>
        /// 
        /// </summary>
        public ReflectionDeferredPassSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
            _renderCandidates = World.GetEntities()
                .With<TransformComponent>()
                .With<PrimitiveComponent>()
                .AsSet();

            _viewBlock = new ShaderViewSpaceBlock();
            _viewDeferredBlock = new ShaderDeferredViewBlock();

            _lightMaterial = new MaterialAsset("DeferredLight") { DepthTest = DepthFunction.Always };
            foreach (var texture in _worldComponents.Get<ReflectionBufferComponent>().DeferredGBuffer.Textures)
                _lightMaterial.SetUniform(texture.Name, texture);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void PreUpdate(bool state)
        {
            var lightCount = World.GetEntities().With<TransformComponent>().With<ReflectionProbeComponent>().AsSet().Count;
            _worldComponents.Get<ReflectionBufferComponent>().ReflectionBlock.Probes = new ShaderReflectionBlock.ShaderReflectionProbe[lightCount];
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, ReadOnlySpan<Entity> entities)
        {
            // GLOBAL PREPERATION
            ref var buffer = ref _worldComponents.Get<ReflectionBufferComponent>();
            var renderCandidates = _renderCandidates.GetEntities();

            Renderer.Use(buffer.DeferredGBuffer);
            Renderer.Use(Defaults.Shader.Program.MeshLitDeferredBuffer);

            GL.Enable(EnableCap.ScissorTest);
            GL.ClearColor(buffer.DeferredGBuffer.ClearColor);

            for (int i = 0; i < entities.Length; i++)
            {
                ref var probeConfig = ref entities[i].Get<ReflectionProbeComponent>();
                ref var transform = ref entities[i].Get<TransformComponent>();

                if (buffer.TextureAtlas.Add(probeConfig.Resolution, out var reflectionMapSpace))
                {
                    probeConfig.InfoID = i;

                    // SHADOW DATA
                    var foo = (probeConfig.Resolution / 3f) % (probeConfig.Resolution / 3) > 0.5f ? 2f : 1f;
                    var widthCorrection = (probeConfig.Resolution - foo) / probeConfig.Resolution;
                    buffer.ReflectionBlock.Probes[i].Area = new Vector4(reflectionMapSpace, reflectionMapSpace.Z * widthCorrection);

                    // RENDER 6 SIDES
                    var cubeOrientations = Helper.CreateCubeOrientations(transform.Position);
                    for (int c = 0; c < cubeOrientations.Length; c++)
                    {
                        // VIEWPORT PREPERATION
                        var viewPort = reflectionMapSpace * new Vector3(
                            buffer.DeferredGBuffer.Width,
                            buffer.DeferredGBuffer.Height,
                            buffer.DeferredGBuffer.Width);
                        var width = (int)viewPort.Z / 3;
                        var height = (int)viewPort.Z / 2;
                        var x = (int)viewPort.X + (c % 3) * width;
                        var y = (int)viewPort.Y + (c < 3 ? 0 : height);
                        GL.Viewport(x, y, width, height);
                        GL.Scissor(x, y, width, height);
                        GL.Clear(buffer.DeferredGBuffer.ClearMask);

                        // VIEW SPACE SETUP
                        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90f), 1f, probeConfig.NearClipping, probeConfig.FarClipping);
                        _viewBlock.WorldToView = cubeOrientations[c];
                        _viewBlock.WorldToProjection = cubeOrientations[c] * projection;
                        _viewBlock.WorldToViewRotation = cubeOrientations[c].ClearScale().ClearTranslation();
                        _viewBlock.WorldToProjectionRotation = cubeOrientations[c].ClearScale().ClearTranslation() * projection;
                        _viewBlock.ViewPosition = new Vector4(transform.Position, 1);
                        _viewBlock.ViewDirection = new Vector4(cubeOrientations[c].Row2);
                        _viewBlock.Resolution = new Vector2(probeConfig.Resolution, probeConfig.Resolution);
                        GPUSync.Push(_viewBlock);

                        // RENDER SCENE
                        Renderer.Use(_viewBlock, Defaults.Shader.Program.MeshLitDeferredBuffer);
                        foreach (ref readonly var candidate in renderCandidates)
                        {
                            var primitive = candidate.Get<PrimitiveComponent>();

                            Renderer.Use(primitive.Material, Defaults.Shader.Program.MeshLitDeferredBuffer);
                            Renderer.Use(primitive.PrimitiveSpaceBlock, Defaults.Shader.Program.MeshLitDeferredBuffer);
                            Renderer.Draw(primitive.Verticies);
                        }
                    }
                }
            }

            Renderer.Use(buffer.DeferredLightBuffer);
            Renderer.Use(Defaults.Shader.Program.MeshLitDeferredLight);

            for (int i = 0; i < entities.Length; i++)
            {
                ref var probeConfig = ref entities[i].Get<ReflectionProbeComponent>();
                ref var transform = ref entities[i].Get<TransformComponent>();

                var bufferSize = new Vector2(buffer.DeferredLightBuffer.Width, buffer.DeferredLightBuffer.Height);
                var viewportStart = buffer.ReflectionBlock.Probes[i].Area.Xy * bufferSize;
                var viewportSize = buffer.ReflectionBlock.Probes[i].Area.Zw * bufferSize;

                _viewDeferredBlock.ViewPosition = new Vector4(transform.Position, 1f);
                _viewDeferredBlock.ViewPort = new Vector4(viewportStart.X, viewportStart.Y, viewportSize.X, viewportSize.Y);
                _viewDeferredBlock.Resolution = bufferSize;
                GPUSync.Push(_viewDeferredBlock);

                GL.Viewport((int)viewportStart.X, (int)viewportStart.Y, (int)viewportSize.X, (int)viewportSize.Y);
                GL.Scissor((int)viewportStart.X, (int)viewportStart.Y, (int)viewportSize.X, (int)viewportSize.Y);
                GL.Clear(buffer.DeferredLightBuffer.ClearMask);

                _lightMaterial.SetUniform("SkyboxMap", probeConfig.Skybox);
                Renderer.Use(_lightMaterial, Defaults.Shader.Program.MeshLitDeferredLight);
                Renderer.Use(_viewDeferredBlock, Defaults.Shader.Program.MeshLitDeferredLight);
                Renderer.Draw(Defaults.Vertex.Mesh.Plane[0]);
            }

            GL.Viewport(0, 0, buffer.Size, buffer.Size);
            GL.Scissor(0, 0, buffer.Size, buffer.Size);
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, buffer.DeferredGBuffer.Handle);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, buffer.DeferredLightBuffer.Handle);
            GL.BlitFramebuffer(
                0, 0, buffer.Size, buffer.Size,
                0, 0, buffer.Size, buffer.Size,
                ClearBufferMask.DepthBufferBit,
                BlitFramebufferFilter.Nearest
            );

            GL.Disable(EnableCap.ScissorTest);
        }
    }
}
