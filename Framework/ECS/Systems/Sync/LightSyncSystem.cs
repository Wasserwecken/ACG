using System;
using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Shader.Block;
using Framework.Assets.Shader.Block.Data;
using Framework.ECS.Components.Light;
using System.Runtime.InteropServices;
using Framework.ECS.Components.Transform;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;


namespace Framework.ECS.Systems.Sync
{
    public class LightSyncSystem : AEntitySetSystem<bool>
    {
        private readonly ShaderBlockArray<ShaderDirectionalLight> _directionalBlock;
        private readonly ShaderBlockArray<ShaderPointLight> _pointBlock;
        private readonly ShaderBlockArray<ShaderSpotLight> _spotBlock;

        private readonly EntitySet _directionalSet;
        private readonly EntitySet _pointSet;
        private readonly EntitySet _spotSet;

        /// <summary>
        /// 
        /// </summary>
        public LightSyncSystem(World world, Entity worldComponents) : base(world)
        {
            _directionalBlock = new ShaderBlockArray<ShaderDirectionalLight>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
            _pointBlock = new ShaderBlockArray<ShaderPointLight>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
            _spotBlock = new ShaderBlockArray<ShaderSpotLight>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);

            _directionalSet = World.GetEntities().With<DirectionalLightComponent>().With<TransformComponent>().AsSet();
            _pointSet = World.GetEntities().With<PointLightComponent>().With<TransformComponent>().AsSet();
            _spotSet = World.GetEntities().With<SpotLightComponent>().With<TransformComponent>().AsSet();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, ReadOnlySpan<Entity> entities)
        {
            _directionalBlock.Data = CreateDirectionalBlock();
            _directionalBlock.PushToGPU();

            _pointBlock.Data = CreatePointBlock();
            _pointBlock.PushToGPU();

            _spotBlock.Data = CreateSpotBlock();
            _spotBlock.PushToGPU();
        }

        /// <summary>
        /// 
        /// </summary>
        private ShaderDirectionalLight[] CreateDirectionalBlock()
        {
            var index = 0;
            var entities = _directionalSet.GetEntities();
            var result = new ShaderDirectionalLight[entities.Length];

            foreach (var entity in entities)
            {
                var light = entity.Get<DirectionalLightComponent>();
                var transform = entity.Get<TransformComponent>();

                result[index].Color = new Vector4(light.Color, light.AmbientFactor);
                result[index].Direction = new Vector4(-transform.Forward, 0f);
                result[index].Shadow = new Vector4(light.ShadowStrength);
                index++;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        private ShaderPointLight[] CreatePointBlock()
        {
            var index = 0;
            var entities = _pointSet.GetEntities();
            var result = new ShaderPointLight[entities.Length];

            foreach (var entity in entities)
            {
                var light = entity.Get<PointLightComponent>();
                var transform = entity.Get<TransformComponent>();

                result[index].Color = new Vector4(light.Color, light.AmbientFactor);
                result[index].Position = new Vector4(transform.Position, 0f);
                index++;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        private ShaderSpotLight[] CreateSpotBlock()
        {
            var index = 0;
            var entities = _spotSet.GetEntities();
            var result = new ShaderSpotLight[entities.Length];

            foreach (var entity in entities)
            {
                var light = entity.Get<SpotLightComponent>();
                var transform = entity.Get<TransformComponent>();

                result[index].Color = new Vector4(light.Color / 30, light.AmbientFactor);
                result[index].Position = new Vector4(transform.Position, MathF.Cos(light.OuterAngle));
                result[index].Direction = new Vector4(-transform.Forward, MathF.Cos(light.InnerAngle));
                index++;
            }

            return result;
        }
    }
}
