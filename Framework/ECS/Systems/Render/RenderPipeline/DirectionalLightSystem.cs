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
            ref var shaderInfo = ref _worldComponents.Get<DirectionalLightInfoComponent>();

            shaderInfo.Data = new ShaderDirectionalLight[entities.Length];
            for (int i = 0; i < entities.Length; i++)
            {
                entities[i].Get<DirectionalLightComponent>().InfoId = i;

                var transform = entities[i].Get<TransformComponent>();
                var config = entities[i].Get<DirectionalLightComponent>();
                SetLightData(ref shaderInfo.Data[i], config, transform);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetLightData(ref ShaderDirectionalLight entry, DirectionalLightComponent config, TransformComponent transform)
        {
            entry.Color = new Vector4(config.Color, config.AmbientFactor);
            entry.Direction = new Vector4(-transform.Forward, 0f);
            entry.ShadowArea = Vector4.Zero;
            entry.ShadowSpace = Matrix4.Zero;
            entry.ShadowStrength = Vector4.Zero;
        }
    }
}
