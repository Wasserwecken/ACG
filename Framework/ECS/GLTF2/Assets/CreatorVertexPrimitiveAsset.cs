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
        public static List<VertexPrimitiveAsset> Create(Mesh gltfMesh)
        {
            var primitives = new List<VertexPrimitiveAsset>();
            
            foreach (var gltfPrimitive in gltfMesh.Primitives)
                primitives.Add(CreatePrimitive(gltfPrimitive));

            return primitives;
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
                        attributes.Add(
                            new VertexAttributeVector3(
                                Definitions.Shader.Attribute.Position.Name,
                                Definitions.Shader.Attribute.Position.Layout,
                                Definitions.Shader.Attribute.Position.Normalize)
                                    { DataTyped = gltfAttribute.Value.AsVector3Array().ToArray().ToOpenTK() }
                        );
                        break;
                    case "NORMAL":
                        attributes.Add(
                            new VertexAttributeVector3(
                                Definitions.Shader.Attribute.Normal.Name,
                                Definitions.Shader.Attribute.Normal.Layout,
                                Definitions.Shader.Attribute.Normal.Normalize)
                            { DataTyped = gltfAttribute.Value.AsVector3Array().ToArray().ToOpenTK() }
                        );
                        break;
                    case "TANGENT":
                        attributes.Add(
                            new VertexAttributeVector4(
                                Definitions.Shader.Attribute.Tangent.Name,
                                Definitions.Shader.Attribute.Tangent.Layout,
                                Definitions.Shader.Attribute.Tangent.Normalize)
                            { DataTyped = gltfAttribute.Value.AsVector4Array().ToArray().ToOpenTK() }
                        );
                        break;
                    case "TEXCOORD_0":
                        attributes.Add(
                            new VertexAttributeVector2(
                                Definitions.Shader.Attribute.UV.Name,
                                Definitions.Shader.Attribute.UV.Layout,
                                Definitions.Shader.Attribute.UV.Normalize)
                            { DataTyped = gltfAttribute.Value.AsVector2Array().ToArray().ToOpenTK() }
                        );
                        break;
                    case "COLOR_0":
                        attributes.Add(
                            new VertexAttributeVector4(
                                Definitions.Shader.Attribute.Color.Name,
                                Definitions.Shader.Attribute.Color.Layout,
                                Definitions.Shader.Attribute.Color.Normalize)
                            { DataTyped = gltfAttribute.Value.AsVector4Array().ToArray().ToOpenTK() }
                        );
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
