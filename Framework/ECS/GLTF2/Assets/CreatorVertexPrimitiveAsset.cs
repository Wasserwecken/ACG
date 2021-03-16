using Framework.Assets.Verticies;
using SharpGLTF.Schema2;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using System.Linq;

namespace Framework.ECS.GLTF2.Assets
{
    public static class CreatorMeshAsset
    {
        /// <summary>
        /// 
        /// </summary>
        public static MeshAsset Create(Mesh gltfMesh)
        {
            var mesh = new MeshAsset(gltfMesh.Name);
            
            foreach (var gltfPrimitive in gltfMesh.Primitives)
                mesh.Primitives.Add(CreatePrimitive(gltfPrimitive));

            return mesh;
        }


        /// <summary>
        /// 
        /// </summary>
        private static VertexPrimitiveAsset CreatePrimitive(MeshPrimitive gltfPrimitive)
        {
            var attributes = new List<VertexAttributeAsset>();
            foreach (var gltfAttribute in gltfPrimitive.VertexAccessors)
            {
                switch (gltfAttribute.Key)
                {
                    case "POSITION":
                        attributes.Add(VertexAttributeAsset.CreatePosition(gltfAttribute.Value.SourceBufferView.Content.ToArray()));
                        break;
                    case "NORMAL":
                        attributes.Add(VertexAttributeAsset.CreateNormal(gltfAttribute.Value.SourceBufferView.Content.ToArray()));
                        break;
                    case "TANGENT":
                        attributes.Add(VertexAttributeAsset.CreateTangent(gltfAttribute.Value.SourceBufferView.Content.ToArray()));
                        break;
                    case "TEXCOORD_0":
                        attributes.Add(VertexAttributeAsset.CreateUV(gltfAttribute.Value.SourceBufferView.Content.ToArray()));
                        break;
                    case "COLOR_0":
                        attributes.Add(VertexAttributeAsset.CreateColor(gltfAttribute.Value.SourceBufferView.Content.ToArray()));
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

            var arrayBuffer = new BufferArrayAsset(BufferUsageHint.StaticDraw, attributes.ToArray());
            var indicieBuffer = new BufferIndicieAsset(BufferUsageHint.StaticDraw, gltfPrimitive.GetIndices().ToArray());
            var primtive = new VertexPrimitiveAsset(arrayBuffer, indicieBuffer);

            return primtive;
        }
    }
}
