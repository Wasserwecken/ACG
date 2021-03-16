using Framework.ECS.Components.Scene;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL;

namespace Framework.ECS.Systems
{
    public class GPUSyncSystem : ISystem
    {
        /// <summary>
        /// 
        /// </summary>
        public void Update(IEnumerable<Entity> entities, IEnumerable<IComponent> sceneComponents)
        {
            var renderGraphComponent = sceneComponents.First(f => f is RenderGraphComponent) as RenderGraphComponent;

            foreach(var primitive in renderGraphComponent.Primitves)
            {
                if (primitive.Handle <= 0)
                {
                    primitive.ArrayBuffer.CreateBufferData();

                    primitive.Handle = GL.GenVertexArray();
                    GL.BindVertexArray(primitive.Handle);


                }
            }
        }
    }
}
