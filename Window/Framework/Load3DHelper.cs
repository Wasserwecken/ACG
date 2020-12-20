using Framework;
using ObjLoader.Loader.Data.Elements;
using ObjLoader.Loader.Loaders;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.IO;

namespace Framework
{
    public static class Load3DHelper
    {
        public static List<MeshObject> Load(string path)
        {
            var objLoaderFactory = new ObjLoaderFactory();
            var objLoader = objLoaderFactory.Create();
            var fileStream = new FileStream(path, FileMode.Open);
            var loadResult = objLoader.Load(fileStream);

            var meshs = new List<MeshObject>();
            foreach(var group in loadResult.Groups)
            {
                var uniqueIds = new Dictionary<FaceVertex, uint>();
                
                var indicies = new List<uint>();
                var verticies = new List<float>();
                var normals = new List<float>();
                var uvs = new List<float>();
                
                foreach(var face in group.Faces)
                {
                    if (face.Count != 3)
                        continue;

                    for(int i = 0; i < 3; i++)
                    {
                        var faceVertex = face[i];

                        if (uniqueIds.TryGetValue(faceVertex, out var index))
                            indicies.Add(index);

                        else
                        {
                            index = (uint)indicies.Count;
                            uniqueIds.Add(faceVertex, index);

                            var vertex = loadResult.Vertices[faceVertex.VertexIndex - 1];
                            verticies.AddRange(new float[] { vertex.X, vertex.Y, vertex.Z });

                            if (faceVertex.NormalIndex > 0)
                            {
                                var normal = loadResult.Normals[faceVertex.NormalIndex - 1];
                                normals.AddRange(new float[] { normal.X, normal.Y, normal.Z });
                            }

                            if (faceVertex.TextureIndex > 0)
                            {
                                var uv = loadResult.Textures[faceVertex.TextureIndex - 1];
                                uvs.AddRange(new float[] { uv.X, uv.Y });
                            }
                        }
                    }
                }

                var mesh = new MeshObject();
                mesh.AddVertices(verticies.ToArray());
                mesh.AddNormals(normals.ToArray());
                mesh.AddUV(uvs.ToArray());
                mesh.AddIndicies(indicies.ToArray());
                mesh.Prepare();

                meshs.Add(mesh);
            }

            return meshs;
        }
    }
}
