using DefaultEcs;
using DefaultEcs.System;
using Framework.ECS.Components.Render;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.ECS.Systems.Render
{
    [With(typeof(RenderPassDataComponent))]
    public class RenderPassCullingSystem : AEntitySetSystem<bool>
    {
        private readonly Entity _worldComponents;

        /// <summary>
        /// 
        /// </summary>
        public RenderPassCullingSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, in Entity entity)
        {
            ref var renderData = ref entity.Get<RenderPassDataComponent>();

            renderData.Renderables.Clear();

            foreach (var candidate in renderData.RenderableCandidates.GetEntities())
                if (!FilterCandidate(candidate, entity))
                    renderData.Renderables.Add(candidate);
        }

        /// <summary>
        /// 
        /// </summary>
        private bool FilterCandidate(Entity candidate, Entity renderSource) => false;
    }
}
