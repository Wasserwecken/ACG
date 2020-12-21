using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Framework
{
    public class MeshObject : VertexObject
    {
        /// <summary>
        /// 
        /// </summary>
        public MeshObject() : base() { }

        /// <summary>
        /// 
        /// </summary>
        public void AddNormals<TType>(ICollection<TType> normals, Func<TType, float[]> toFloats)
        {
            AddAttribute(
                Definitions.Shader.Attributes.Normals.LAYOUT,
                Definitions.Buffer.Normals.PAIRLENGTH,
                Definitions.Buffer.Normals.NORMALIZED,
                Definitions.Buffer.Normals.POINTERTYPE,
                normals, toFloats
            );
        }

        /// <summary>
        /// 
        /// </summary>
        public void AddNormals(float[] normals)
        {
            AddAttribute(
                Definitions.Shader.Attributes.Normals.LAYOUT,
                Definitions.Buffer.Normals.PAIRLENGTH,
                Definitions.Buffer.Normals.NORMALIZED,
                Definitions.Buffer.Normals.POINTERTYPE,
                normals
            );
        }

        /// <summary>
        /// 
        /// </summary>
        public void AddUV<TType>(ICollection<TType> uv, Func<TType, float[]> toFloats)
        {
            AddAttribute(
                Definitions.Shader.Attributes.UV.LAYOUT,
                Definitions.Buffer.UV.PAIRLENGTH,
                Definitions.Buffer.UV.NORMALIZED,
                Definitions.Buffer.UV.POINTERTYPE,
                uv, toFloats
            );
        }

        /// <summary>
        /// 
        /// </summary>
        public void AddUV(float[] uv)
        {
            AddAttribute(
                Definitions.Shader.Attributes.UV.LAYOUT,
                Definitions.Buffer.UV.PAIRLENGTH,
                Definitions.Buffer.UV.NORMALIZED,
                Definitions.Buffer.UV.POINTERTYPE,
                uv
            );
        }
    }
}
