using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;


namespace Framework
{
    public static class VertexObjectSystem
    {
        /// <summary>
        /// 
        /// </summary>
        public static void PushToGPU(VertexObjectAsset vertexObject)
        {
            foreach (var primitive in vertexObject.Primitives)
                VertexPrimitiveSystem.PushToGPU(primitive);
        }
    }
}
