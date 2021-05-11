using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Shader.Block.Data;
using Framework.ECS.Components.Light;
using Framework.ECS.Components.Render;
using Framework.ECS.Components.Transform;
using OpenTK.Mathematics;
using System;

namespace Framework.ECS.Systems.RenderPipeline
{
    [With(typeof(TransformComponent))]
    [With(typeof(DirectionalLightComponent))]
    public class DirectionalLightSystem : AEntitySetSystem<bool>
    {
        private readonly Entity _worldComponents;


        /// <summary>
        /// 
        /// </summary>
        public DirectionalLightSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, ReadOnlySpan<Entity> entities)
        {
            ref var shaderBlocks = ref _worldComponents.Get<GlobalShaderBlocksComponent>();
            shaderBlocks.DirectionalLights.Data = new ShaderDirectionalLight[entities.Length];

            for (int i = 0; i < entities.Length; i++)
            {
                var transform = entities[i].Get<TransformComponent>();
                var lightConfig = entities[i].Get<DirectionalLightComponent>();

                entities[i].Get<DirectionalLightComponent>().InfoId = i;

                shaderBlocks.DirectionalLights.Data[i].Color = new Vector4(lightConfig.Color, lightConfig.AmbientFactor);
                shaderBlocks.DirectionalLights.Data[i].Direction = new Vector4(-transform.Forward, 0f);
                shaderBlocks.DirectionalLights.Data[i].ShadowArea = Vector4.Zero;
                shaderBlocks.DirectionalLights.Data[i].ShadowSpace = Matrix4.Zero;
                shaderBlocks.DirectionalLights.Data[i].ShadowStrength = Vector4.Zero;
            }

            shaderBlocks.DirectionalLights.PushToGPU();
        }
    }
}
