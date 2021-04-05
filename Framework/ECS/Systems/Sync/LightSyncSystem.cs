using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Assets.Shader.Block;
using Framework.Assets.Shader.Block.Data;
using Framework.Assets.Textures;
using Framework.ECS.Components.Light;
using Framework.ECS.Components.Transform;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;


namespace Framework.ECS.Systems.Sync
{
    public class LightSyncSystem : ISystem
    {
        private readonly ShaderBlockArray<ShaderDirectionalLight> _directionalBlock;
        private readonly ShaderBlockArray<ShaderPointLight> _pointBlock;
        private readonly ShaderBlockArray<ShaderSpotLight> _spotBlock;

        /// <summary>
        /// 
        /// </summary>
        public LightSyncSystem()
        {
            _directionalBlock = new ShaderBlockArray<ShaderDirectionalLight>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
            _pointBlock = new ShaderBlockArray<ShaderPointLight>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
            _spotBlock = new ShaderBlockArray<ShaderSpotLight>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Run(IEnumerable<Entity> entities, IEnumerable<IComponent> sceneComponents)
        {
            _directionalBlock.Data = CreateDirectionalBlock(entities);
            _directionalBlock.PushToGPU();

            _pointBlock.Data = CreatePointBlock(entities);
            _pointBlock.PushToGPU();

            _spotBlock.Data = CreateSpotBlock(entities);
            _spotBlock.PushToGPU();
        }

        /// <summary>
        /// 
        /// </summary>
        private static ShaderDirectionalLight[] CreateDirectionalBlock(IEnumerable<Entity> entities)
        {
            var index = 0;
            var directionals = entities.Where(f => f.Components.HasAll(typeof(DirectionalLightComponent), typeof(TransformComponent)));
            var result = new ShaderDirectionalLight[directionals.Count()];

            foreach (var entity in directionals)
            {
                var light = entity.Components.Get<DirectionalLightComponent>();
                var transform = entity.Components.Get<TransformComponent>();

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
        private static ShaderPointLight[] CreatePointBlock(IEnumerable<Entity> entities)
        {
            var index = 0;
            var points = entities.Where(f => f.Components.HasAll(typeof(PointLightComponent), typeof(TransformComponent)));
            var result = new ShaderPointLight[points.Count()];

            foreach (var entity in points)
            {
                var light = entity.Components.Get<PointLightComponent>();
                var transform = entity.Components.Get<TransformComponent>();

                result[index].Color = new Vector4(light.Color / 30, light.AmbientFactor);
                result[index].Position = new Vector4(transform.Position, 0f);
                index++;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        private static ShaderSpotLight[] CreateSpotBlock(IEnumerable<Entity> entities)
        {
            var index = 0;
            var spots = entities.Where(f => f.Components.HasAll(typeof(SpotLightComponent), typeof(TransformComponent)));
            var result = new ShaderSpotLight[spots.Count()];

            foreach (var entity in spots)
            {
                var light = entity.Components.Get<SpotLightComponent>();
                var transform = entity.Components.Get<TransformComponent>();

                var foo = transform.Scale;

                result[index].Color = new Vector4(light.Color / 30, light.AmbientFactor);
                result[index].Position = new Vector4(transform.Position, MathF.Cos(light.OuterAngle));
                result[index].Direction = new Vector4(-transform.Forward, MathF.Cos(light.InnerAngle));
                index++;
            }

            return result;
        }
    }
}
