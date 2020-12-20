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
        public void AddVertices<TType>(ICollection<TType> vertices, Func<TType, float[]> toFloats)
        {
            AddAttribute(
                Definitions.Shader.Attributes.Vertices.LAYOUT,
                Definitions.Buffer.Vertices.PAIRLENGTH,
                Definitions.Buffer.Vertices.NORMALIZED,
                Definitions.Buffer.Vertices.POINTERTYPE,
                vertices, toFloats
            );
        }

        /// <summary>
        /// 
        /// </summary>
        public void AddVertices(float[] vertices)
        {
            AddAttribute(
                Definitions.Shader.Attributes.Vertices.LAYOUT,
                Definitions.Buffer.Vertices.PAIRLENGTH,
                Definitions.Buffer.Vertices.NORMALIZED,
                Definitions.Buffer.Vertices.POINTERTYPE,
                vertices
            );
        }

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
        public void AddColors<TType>(ICollection<TType> colors, Func<TType, float[]> toFloats)
        {
            AddAttribute(
                Definitions.Shader.Attributes.Colors.LAYOUT,
                Definitions.Buffer.Colors.PAIRLENGTH,
                Definitions.Buffer.Colors.NORMALIZED,
                Definitions.Buffer.Colors.POINTERTYPE,
                colors, toFloats
            );
        }

        /// <summary>
        /// 
        /// </summary>
        public void AddColors(float[] colors)
        {
            AddAttribute(
                Definitions.Shader.Attributes.Colors.LAYOUT,
                Definitions.Buffer.Colors.PAIRLENGTH,
                Definitions.Buffer.Colors.NORMALIZED,
                Definitions.Buffer.Colors.POINTERTYPE,
                colors
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
