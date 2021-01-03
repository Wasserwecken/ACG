using System;
using System.Collections.Generic;
using System.Text;
using SharpGLTF.Schema2;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Linq;

namespace Framework
{
    public static class GLTF2System
    {
        /// <summary>
        /// 
        /// </summary>
        public static void Foo(string filePath)
        {
            var glTF = ModelRoot.Load(filePath);

            GenerateVertexObjects(glTF.LogicalMeshes, out var generatedMeshs);
            GenerateMaterialObjects(glTF.LogicalMaterials, out var generatedMaterials);

        }

        /// <summary>
        /// 
        /// </summary>
        private static void GenerateVertexObjects(IEnumerable<Mesh> gltfMeshs, out Dictionary<Mesh, VertexObjectAsset> result)
        {
            result = new Dictionary<Mesh, VertexObjectAsset>();

            foreach (var glTFmesh in gltfMeshs)
            {
                var primitives = new List<VertexPrimitiveAsset>();

                foreach (var glTFprimitive in glTFmesh.Primitives)
                {
                    var vertexAttributes = new List<VertexAttributeAsset>();
                    foreach (var glTFaccessor in glTFprimitive.VertexAccessors)
                    {
                        var attributeAsset = new VertexAttributeAsset(
                            glTFaccessor.Key,
                            glTFaccessor.Value.LogicalIndex,
                            glTFaccessor.Value.Format.ByteSize,
                            glTFaccessor.Value.Format.Normalized,
                            (VertexAttribPointerType)glTFaccessor.Value.Format.Encoding
                        );

                        VertexAttributeSystem.Update(
                            attributeAsset,
                            glTFaccessor.Value.SourceBufferView.Content.ToArray()
                        );

                        vertexAttributes.Add(attributeAsset);
                    }

                    var arrayBuffer = new ArrayBufferAsset(vertexAttributes.ToArray(), BufferUsageHint.StaticDraw);
                    ArrayBufferSystem.Update(arrayBuffer);

                    var indicieBuffer = new IndicieBufferAsset(BufferUsageHint.StaticDraw);
                    IndicieBufferSystem.Update(glTFprimitive.GetIndices().ToArray(), indicieBuffer);

                    primitives.Add(new VertexPrimitiveAsset(arrayBuffer, indicieBuffer)
                    {
                        Mode = PolygonMode.Fill,
                        Type = (OpenTK.Graphics.OpenGL.PrimitiveType)glTFprimitive.DrawPrimitiveType
                    });
                }

                result.Add(glTFmesh, new VertexObjectAsset(glTFmesh.Name, primitives.ToArray()));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static void GenerateMaterialObjects(IEnumerable<Material> gltfMaterials, out Dictionary<Material, MaterialAsset> result)
        {
            result = new Dictionary<Material, MaterialAsset>();

            foreach(var gltfMaterial in gltfMaterials)
            {
                result.Add(gltfMaterial, new MaterialAsset(default));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void GenerateEntities(IEnumerable<Node> gltfSceneNodes, out List<Entity> entities)
        {
            entities = new List<Entity>();


        }
    }
}
