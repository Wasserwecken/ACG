using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Framework
{
    public class TransformIndicatorObject : VertexObject
    {
        /// <summary>
        /// 
        /// </summary>
        public TransformIndicatorObject() : base()
        {
            Primitive = PrimitiveType.Lines;

            AddVertices(new float[]
            {
                0f, 0f, 0f,
                1f, 0f, 0f,
                0f, 1f, 0f,
                0f, 0f, 1f
            });

            AddAttribute(1, 4, false, VertexAttribPointerType.Float, new float[]
            {
                1f, 1f, 1f, 0.8f,
                1f, 0f, 0f, 0.8f,
                0f, 1f, 0f, 0.8f,
                0f, 0f, 1f, 0.8f
            });

            AddIndicies(new uint[]
            {
                0, 1, 0, 2, 0, 3
            });

            Prepare();
            PushToGPU();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void DrawObject()
        {
            GL.DepthMask(false);
            GL.Disable(EnableCap.DepthTest);

            GL.LineWidth(5f);
            base.DrawObject();

            GL.DepthMask(true);
            GL.Enable(EnableCap.DepthTest);
        }
    }
}
