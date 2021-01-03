using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public static class VertexAttributeSystem
    {
        /// <summary>
        /// 
        /// </summary>
        public static void Update(VertexAttributeAsset attribute, byte[] data)
        {
            attribute.Data = data;
            attribute.ElementCount = data.Length / attribute.ElementSize;
        }
    }
}
