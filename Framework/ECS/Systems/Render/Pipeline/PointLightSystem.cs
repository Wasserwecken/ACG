using ACG.Framework.Assets;
using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Shader.Block;
using Framework.ECS.Components.Light;
using Framework.ECS.Components.Transform;
using Framework.ECS.Systems.Render.OpenGL;
using OpenTK.Mathematics;
using System;

namespace Framework.ECS.Systems.Render.Pipeline
{
    [With(typeof(TransformComponent))]
    [With(typeof(PointLightComponent))]
    public class PointLightSystem : AEntitySetSystem<bool>
    {
        private readonly Entity _worldComponents;
        private readonly ShaderPointLightBlock _block;

        /// <summary>
        /// 
        /// </summary>
        public PointLightSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
            _block = new ShaderPointLightBlock();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, ReadOnlySpan<Entity> entities)
        {
            _block.Lights = new ShaderPointLightBlock.ShaderDirectionalLight[entities.Length];
            for (int i = 0; i < entities.Length; i++)
            {
                var transform = entities[i].Get<TransformComponent>();
                var lightConfig = entities[i].Get<PointLightComponent>();

                entities[i].Get<PointLightComponent>().InfoId = i;

                _block.Lights[i].Color = new Vector4(lightConfig.Color, lightConfig.AmbientFactor);
                _block.Lights[i].Position = new Vector4(transform.Position, lightConfig.Range);
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
