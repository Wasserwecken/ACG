using DefaultEcs;
using DefaultEcs.System;
using Framework.ECS.Components.Render;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.ECS.Systems.Render
{
    [With(typeof(RenderPassViewComponent))]
    [With(typeof(RenderPassGraphComponent))]
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
            var renderConfig = entity.Get<RenderPassViewComponent>();
            var renderGraph = entity.Get<RenderPassGraphComponent>();

            renderGraph.Renderables.Clear();

            foreach (var candidate in renderConfig.RenderableCandidates.GetEntities())
                if (!FilterCandidate(candidate, entity))
                    renderGraph.Renderables.Add(candidate);

            entity.Set(renderGraph);
        }

        /// <summary>
        /// 
        /// </summary>
        private bool FilterCandidate(Entity candidate, Entity renderSource) => false;
    }
}
