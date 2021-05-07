using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Shader.Block;
using Framework.Assets.Shader.Block.Data;
using Framework.ECS.Components.Light;
using Framework.ECS.Components.Transform;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;

namespace Framework.ECS.Systems.RenderPipeline
{
    [With(typeof(TransformComponent))]
    [With(typeof(SpotLightComponent))]
    public class SpotLightSystem : AEntitySetSystem<bool>
    {
        private readonly Entity _worldComponents;
        private readonly ShaderBlockArray<ShaderSpotLight> _spotInfoBlock;

        /// <summary>
        /// 
        /// </summary>
        public SpotLightSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
            _spotInfoBlock = new ShaderBlockArray<ShaderSpotLight>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, ReadOnlySpan<Entity> entities)
        {
            ref var shaderInfo = ref _worldComponents.Get<SpotLightCollectionComponent>();

            shaderInfo.Data = new ShaderSpotLight[entities.Length];
            for (int i = 0; i < entities.Length; i++)
            {
                var transform = entities[i].Get<TransformComponent>();
                var lightConfig = entities[i].Get<SpotLightComponent>();

                entities[i].Get<SpotLightComponent>().InfoId = i;

                shaderInfo.Data[i].Color = new Vector4(lightConfig.Color, lightConfig.AmbientFactor);
                shaderInfo.Data[i].Position = new Vector4(transform.Position, MathF.Cos(lightConfig.OuterAngle));
                shaderInfo.Data[i].Direction = new Vector4(-transform.Forward, MathF.Cos(lightConfig.InnerAngle));
            }

            _spotInfoBlock.Data = shaderInfo.Data;
            _spotInfoBlock.PushToGPU();
        }
    }
}
