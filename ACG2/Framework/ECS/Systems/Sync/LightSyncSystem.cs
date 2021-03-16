using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Assets.Shader.Block;
using Framework.ECS.Components.Light;
using Framework.ECS.Components.Transform;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;


namespace Framework.ECS.Systems.Sync
{
    public class LightSyncSystem : ISystem
    {
        ShaderBlockArray<ShaderDirectionalLight> _directionalLightBlock;
        ShaderBlockArray<ShaderPointLight> _pointLightBlock;
        ShaderBlockArray<ShaderSpotLight> _spotLightBlock;

        public LightSyncSystem()
        {
            _directionalLightBlock = new ShaderBlockArray<ShaderDirectionalLight>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
            _pointLightBlock = new ShaderBlockArray<ShaderPointLight>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
            _spotLightBlock = new ShaderBlockArray<ShaderSpotLight>(BufferRangeTarget.ShaderStorageBuffer, BufferUsageHint.DynamicDraw);
        }

        public void Run(IEnumerable<Entity> entities, IEnumerable<IComponent> sceneComponents)
        {
            int index;


            index = 0;
            var directional = entities.Where(f => f.HasAllComponents(typeof(DirectionalLightComponent), typeof(TransformComponent)));
            _directionalLightBlock.Data = new ShaderDirectionalLight[directional.Count()];
            foreach (var entity in directional)
            {
                var light = entity.GetComponent<DirectionalLightComponent>();
                var transform = entity.GetComponent<TransformComponent>();

                _directionalLightBlock.Data[index].Color = new Vector4(light.Color, light.AmbientFactor);
                _directionalLightBlock.Data[index].Direction = new Vector4(transform.Forward, 0f);
                index++;
            }
            _directionalLightBlock.PushToGPU();


            index = 0;
            var points = entities.Where(f => f.HasAllComponents(typeof(PointLightComponent), typeof(TransformComponent)));
            _pointLightBlock.Data = new ShaderPointLight[points.Count()];
            foreach (var entity in points)
            {
                var light = entity.GetComponent<PointLightComponent>();
                var transform = entity.GetComponent<TransformComponent>();

                _pointLightBlock.Data[index].Color = new Vector4(light.Color / 30, light.AmbientFactor);
                _pointLightBlock.Data[index].Position = new Vector4(transform.Position, 1f);
                index++;
            }
            _pointLightBlock.PushToGPU();


            index = 0;
            var spots = entities.Where(f => f.HasAllComponents(typeof(SpotLightComponent), typeof(TransformComponent)));
            _spotLightBlock.Data = new ShaderSpotLight[spots.Count()];
            foreach (var entity in spots)
            {
                var light = entity.GetComponent<SpotLightComponent>();
                var transform = entity.GetComponent<TransformComponent>();

                var foo = transform.Scale;

                _spotLightBlock.Data[index].Color = new Vector4(light.Color / 30, light.AmbientFactor);
                _spotLightBlock.Data[index].Position = new Vector4(transform.Position, MathF.Cos(light.OuterAngle));
                _spotLightBlock.Data[index].Direction = new Vector4(transform.Forward, MathF.Cos(light.InnerAngle));
                index++;
            }
            _spotLightBlock.PushToGPU();
        }
    }
}
