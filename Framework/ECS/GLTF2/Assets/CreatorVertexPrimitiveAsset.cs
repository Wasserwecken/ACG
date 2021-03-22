using Framework.Assets.Verticies;
using SharpGLTF.Schema2;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using System.Linq;
using Framework.Assets.Verticies.Attributes;
using Framework.Extensions;

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
            var attributes = new List<IVertexAttribute>();
            foreach (var gltfAttribute in gltfPrimitive.VertexAccessors)
            {
                switch (gltfAttribute.Key)
                {
                    case "POSITION":
                        attributes.Add(new VertexAttributePositionAsset(gltfAttribute.Value.AsVector3Array().ToArray().ToOpenTK()));
                        break;
                    case "NORMAL":
                        attributes.Add(new VertexAttributeNormalAsset(gltfAttribute.Value.AsVector3Array().ToArray().ToOpenTK()));
                        break;
                    case "TANGENT":
                        attributes.Add(new VertexAttributeTangentAsset(gltfAttribute.Value.AsVector4Array().ToArray().ToOpenTK()));
                        break;
                    case "TEXCOORD_0":
                        attributes.Add(new VertexAttributeUVAsset(gltfAttribute.Value.AsVector2Array().ToArray().ToOpenTK()));
                        break;
                    case "COLOR_0":
                        attributes.Add(new VertexAttributeColorAsset(gltfAttribute.Value.AsVector4Array().ToArray().ToOpenTK()));
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
