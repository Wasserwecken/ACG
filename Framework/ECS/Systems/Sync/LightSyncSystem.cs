using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Assets.Shader.Block;
using Framework.Assets.Shader.Block.Data;
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

        public LightSyncSystem()
        {
            _directionalBlock = new ShaderBlockArray<ShaderDirectionalLight>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
            _pointBlock = new ShaderBlockArray<ShaderPointLight>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
            _spotBlock = new ShaderBlockArray<ShaderSpotLight>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
        }

        public void Run(IEnumerable<Entity> entities, IEnumerable<IComponent> sceneComponents)
        {
            int index;


            index = 0;
            var directional = entities.Where(f => f.HasAllComponents(typeof(DirectionalLightComponent), typeof(TransformComponent)));
            _directionalBlock.Data = new ShaderDirectionalLight[directional.Count()];
            foreach (var entity in directional)
            {
                var light = entity.GetComponent<DirectionalLightComponent>();
                var transform = entity.GetComponent<TransformComponent>();

                _directionalBlock.Data[index].Color = new Vector4(light.Color, light.AmbientFactor);
                _directionalBlock.Data[index].Direction = new Vector4(transform.Forward, 0f);
                index++;
            }
            _directionalBlock.PushToGPU();


            index = 0;
            var points = entities.Where(f => f.HasAllComponents(typeof(PointLightComponent), typeof(TransformComponent)));
            _pointBlock.Data = new ShaderPointLight[points.Count()];
            foreach (var entity in points)
            {
                var light = entity.GetComponent<PointLightComponent>();
                var transform = entity.GetComponent<TransformComponent>();

                _pointBlock.Data[index].Color = new Vector4(light.Color / 30, light.AmbientFactor);
                _pointBlock.Data[index].Position = new Vector4(transform.Position, 1f);
                index++;
            }
            _pointBlock.PushToGPU();


            index = 0;
            var spots = entities.Where(f => f.HasAllComponents(typeof(SpotLightComponent), typeof(TransformComponent)));
            _spotBlock.Data = new ShaderSpotLight[spots.Count()];
            foreach (var entity in spots)
            {
                var light = entity.GetComponent<SpotLightComponent>();
                var transform = entity.GetComponent<TransformComponent>();

                var foo = transform.Scale;

                _spotBlock.Data[index].Color = new Vector4(light.Color / 30, light.AmbientFactor);
                _spotBlock.Data[index].Position = new Vector4(transform.Position, MathF.Cos(light.OuterAngle));
                _spotBlock.Data[index].Direction = new Vector4(transform.Forward, MathF.Cos(light.InnerAngle));
                index++;
            }
            _spotBlock.PushToGPU();
        }
    }
}
