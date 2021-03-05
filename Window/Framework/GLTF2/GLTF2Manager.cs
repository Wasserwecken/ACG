using SharpGLTF.Schema2;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using System.Linq;

namespace Framework
{
    public static class GLTF2Manager
    {
        /// <summary>
        /// 
        /// </summary>
        public static VertexPrimitiveAsset CreatePrimitive(MeshPrimitive gltfPrimitive)
        {

            var attributes = new List<VertexAttributeAsset>();
            foreach(var gltfAttribute in gltfPrimitive.VertexAccessors)
            {
                switch(gltfAttribute.Key)
                {
                    case "POSITION":
                        attributes.Add(VertexAttributeManager.CreatePosition(gltfAttribute.Value.SourceBufferView.Content.ToArray()));
                        break;
                    case "NORMAL":
                        attributes.Add(VertexAttributeManager.CreateNormal(gltfAttribute.Value.SourceBufferView.Content.ToArray()));
                        break;
                    case "TANGENT":
                        attributes.Add(VertexAttributeManager.CreateTangent(gltfAttribute.Value.SourceBufferView.Content.ToArray()));
                        break;
                    case "TEXCOORD_0":
                        attributes.Add(VertexAttributeManager.CreateUV(gltfAttribute.Value.SourceBufferView.Content.ToArray()));
                        break;
                    case "COLOR_0":
                        attributes.Add(VertexAttributeManager.CreateColor(gltfAttribute.Value.SourceBufferView.Content.ToArray()));
                        break;

                    default:
                        attributes.Add(new VertexAttributeAsset(
                            gltfAttribute.Key,
                            gltfAttribute.Value.LogicalIndex,
                            gltfAttribute.Value.Format.ByteSize,
                            gltfAttribute.Value.Format.Normalized,
                            (VertexAttribPointerType)gltfAttribute.Value.Format.Encoding,
                            gltfAttribute.Value.SourceBufferView.Content.ToArray()
                        ));
                        break;
                }
            }

            var arrayBuffer = new ArrayBufferAsset(BufferUsageHint.StaticDraw, attributes.ToArray());
            var indicieBuffer = new IndicieBufferAsset(BufferUsageHint.StaticDraw, gltfPrimitive.GetIndices().ToArray());
            var primtive = new VertexPrimitiveAsset(arrayBuffer, indicieBuffer);

            return primtive;
        }
    }
}
