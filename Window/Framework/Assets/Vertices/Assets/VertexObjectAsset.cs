using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    [DebuggerDisplay("Name: {Name}, Primitives: {Primitives.Length}")]
    public class VertexObjectAsset
    {
        public string Name { get; }
        public VertexPrimitiveAsset[] Primitives { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public VertexObjectAsset(string name, VertexPrimitiveAsset[] primitives)
        {
            Name = name;
            Primitives = primitives;
        }
    }
}
