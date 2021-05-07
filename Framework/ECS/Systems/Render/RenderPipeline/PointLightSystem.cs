using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Shader.Block.Data;
using Framework.ECS.Components.Light;
using Framework.ECS.Components.Transform;
using OpenTK.Mathematics;
using System;

namespace Framework.ECS.Systems.RenderPipeline
{
    [With(typeof(TransformComponent))]
    [With(typeof(PointLightComponent))]
    public class PointLightSystem : AEntitySetSystem<bool>
    {
        private readonly Entity _worldComponents;

        /// <summary>
        /// 
        /// </summary>
        public PointLightSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, ReadOnlySpan<Entity> entities)
        {
            ref var shaderInfo = ref _worldComponents.Get<PointLightCollectionComponent>();

            shaderInfo.Data = new ShaderPointLight[entities.Length];
            for (int i = 0; i < entities.Length; i++)
            {
                var transform = entities[i].Get<TransformComponent>();
                var lightConfig = entities[i].Get<PointLightComponent>();

                entities[i].Get<PointLightComponent>().InfoId = i;

                shaderInfo.Data[i].Color = new Vector4(lightConfig.Color, lightConfig.AmbientFactor);
                shaderInfo.Data[i].Position = new Vector4(transform.Position, 0f);
                shaderInfo.Data[i].ShadowArea = Vector4.Zero;
                shaderInfo.Data[i].ShadowSpace = Matrix4.Zero;
                shaderInfo.Data[i].ShadowStrength = Vector4.Zero;
            }
        }
    }
}
