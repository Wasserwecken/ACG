using DefaultEcs;
using DefaultEcs.System;
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

        /// <summary>
        /// 
        /// </summary>
        public ReflectionDeferredPassSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
            _viewBlock = new ShaderViewSpaceBlock();
            _renderCandidates = World.GetEntities()
                .With<TransformComponent>()
                .With<PrimitiveComponent>()
                .AsSet();
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
            ref var reflection = ref _worldComponents.Get<ReflectionBufferComponent>();

            Renderer.UseFrameBuffer(reflection.DeferredBuffer);
            Renderer.UseShader(Defaults.Shader.Program.MeshBlinnPhongDeferredBuffer);

            GL.Enable(EnableCap.ScissorTest);
            GL.ClearColor(reflection.DeferredBuffer.ClearColor);
            GL.ClearColor(Color4.Aqua);

            for(int i = 0; i < entities.Length; i++)
            {
                // DATA COLLECTION
                var transform = entities[i].Get<TransformComponent>();
                ref var probeConfig = ref entities[i].Get<ReflectionProbeComponent>();

                if (reflection.TextureAtlas.Add(probeConfig.Resolution, out var reflectionMapSpace))
                {
                    // SHADOW DATA
                    var foo = (probeConfig.Resolution / 3f) % (probeConfig.Resolution / 3) > 0.5f ? 2f : 1f;
                    var widthCorrection = (probeConfig.Resolution - foo) / probeConfig.Resolution;
                    reflection.ReflectionBlock.Probes[i].Area = new Vector4(reflectionMapSpace, reflectionMapSpace.Z * widthCorrection);

                    // RENDER 6 SIDES
                    var renderCandidates = _renderCandidates.GetEntities();
                    var cubeOrientations = Helper.CreateCubeOrientations(transform.Position);
                    for (int c = 0; c < cubeOrientations.Length; c++)
                    {
                        // VIEWPORT PREPERATION
                        var viewPort = reflectionMapSpace * new Vector3(
                            reflection.DeferredBuffer.Width,
                            reflection.DeferredBuffer.Height,
                            reflection.DeferredBuffer.Width);
                        var width = (int)viewPort.Z / 3;
                        var height = (int)viewPort.Z / 2;
                        var x = (int)viewPort.X + (c % 3) * width;
                        var y = (int)viewPort.Y + (c < 3 ? 0 : height);
                        GL.Viewport(x, y, width, height);
                        GL.Scissor(x, y, width, height);
                        GL.Clear(reflection.DeferredBuffer.ClearMask);

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
                        Renderer.UseShaderBlock(_viewBlock, Defaults.Shader.Program.MeshBlinnPhongDeferredBuffer);
                        foreach (ref readonly var candidate in renderCandidates)
                        {
                            var primitive = candidate.Get<PrimitiveComponent>();

                            Renderer.UseMaterial(primitive.Material, Defaults.Shader.Program.MeshBlinnPhongDeferredBuffer);
                            Renderer.UseShaderBlock(primitive.ShaderSpaceBlock, Defaults.Shader.Program.MeshBlinnPhongDeferredBuffer);
                            Renderer.Draw(primitive.Verticies);
                        }
                    }

                    probeConfig.HasChanged = false;
                }
            }

            GL.Disable(EnableCap.ScissorTest);
        }
    }
}
