using ACG.Framework.Assets;
using DefaultEcs;
using DefaultEcs.System;
using Framework.Assets.Shader.Block;
using Framework.ECS.Components.Light;
using Framework.ECS.Components.Transform;
using Framework.ECS.Systems.Render.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Diagnostics;

namespace Framework.ECS.Systems.Render.Pipeline
{
    [With(typeof(TransformComponent))]
    [With(typeof(DirectionalLightComponent))]
    public class DirectionalLightSystem : AEntitySetSystem<bool>
    {
        private readonly Entity _worldComponents;
        private readonly ShaderDirectionalLightBlock _block;


        /// <summary>
        /// 
        /// </summary>
        public DirectionalLightSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
            _block = new ShaderDirectionalLightBlock();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, ReadOnlySpan<Entity> entities)
        {
            _block.Lights = new ShaderDirectionalLightBlock.ShaderDirectionalLight[entities.Length];
            for (int i = 0; i < entities.Length; i++)
            {
                var transform = entities[i].Get<TransformComponent>();
                var lightConfig = entities[i].Get<DirectionalLightComponent>();

                entities[i].Get<DirectionalLightComponent>().InfoId = i;

                _block.Lights[i].Color = new Vector4(lightConfig.Color, lightConfig.AmbientFactor);
                _block.Lights[i].Direction = new Vector4(-transform.Forward, 0f);
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
