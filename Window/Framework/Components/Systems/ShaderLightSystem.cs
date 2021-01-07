using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public static class ShaderLightSystem
    {
        /// <summary>
        /// 
        /// </summary>
        public static void Copy(Entity[] entities, ref ShaderDirectionalLight[] shaderLight)
        {
            for (int i = 0; i < entities.Length; i++)
            {
                entities[i].TryGetComponent<DirectionalLightComponent>(out var light);
                entities[i].TryGetComponent<WorldTransformComponent>(out var transform);

                shaderLight[i].Color = new Vector4(light.Color, light.AmbientFactor);
                shaderLight[i].Direction = new Vector4(transform.Forward, 0f);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Copy(Entity[] entities, ref ShaderPointLight[] shaderLight)
        {
            for (int i = 0; i < entities.Length; i++)
            {
                entities[i].TryGetComponent<PointLightComponent>(out var light);
                entities[i].TryGetComponent<WorldTransformComponent>(out var transform);

                shaderLight[i].Color = new Vector4(light.Color / 30, light.AmbientFactor);
                shaderLight[i].Position = new Vector4(transform.Position, 0f);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Copy(Entity[] entities, ref ShaderSpotLight[] shaderLight)
        {
            for (int i = 0; i < entities.Length; i++)
            {
                entities[i].TryGetComponent<SpotLightComponent>(out var light);
                entities[i].TryGetComponent<WorldTransformComponent>(out var transform);

                shaderLight[i].Color = new Vector4(light.Color / 30, light.AmbientFactor);
                shaderLight[i].Position = new Vector4(transform.Position, MathF.Cos(light.OuterAngle));
                shaderLight[i].Direction = new Vector4(transform.Forward, MathF.Cos(light.InnerAngle));
            }
        }
    }
}
