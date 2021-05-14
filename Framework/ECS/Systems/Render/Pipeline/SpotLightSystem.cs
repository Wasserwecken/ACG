using ACG.Framework.Assets;
using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Shader.Block;
using Framework.ECS.Components.Light;
using Framework.ECS.Components.Transform;
using Framework.ECS.Systems.Render.OpenGL;
using OpenTK.Mathematics;
using System;

namespace Framework.ECS.Systems.RenderPipeline
{
    [With(typeof(TransformComponent))]
    [With(typeof(SpotLightComponent))]
    public class SpotLightSystem : AEntitySetSystem<bool>
    {
        private readonly Entity _worldComponents;
        private readonly ShaderSpotLightBlock _block;

        /// <summary>
        /// 
        /// </summary>
        public SpotLightSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
            _block = new ShaderSpotLightBlock();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, ReadOnlySpan<Entity> entities)
        {
            _block.Lights = new ShaderSpotLightBlock.ShaderSpotLight[entities.Length];
            for (int i = 0; i < entities.Length; i++)
            {
                var transform = entities[i].Get<TransformComponent>();
                var lightConfig = entities[i].Get<SpotLightComponent>();

                entities[i].Get<SpotLightComponent>().InfoId = i;

                _block.Lights[i].Color = new Vector4(lightConfig.Color, lightConfig.AmbientFactor);
                _block.Lights[i].Position = new Vector4(transform.Position, MathF.Cos(lightConfig.OuterAngle));
                _block.Lights[i].Direction = new Vector4(-transform.Forward, MathF.Cos(lightConfig.InnerAngle));
                _block.Lights[i].Range = new Vector4(lightConfig.Range, 0f, 0f, 0f);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void PostUpdate(bool state)
        {
            GPUSync.Push(_block);

            foreach (var shader in AssetRegister.Shaders)
                shader.SetBlockBinding(_block);
        }
    }
}
