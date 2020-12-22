using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public abstract class CameraBase
    {
        public CameraData BaseData { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Use(ref RenderData renderData)
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Clear(BaseData.ClearMask);
        }
    }
}
