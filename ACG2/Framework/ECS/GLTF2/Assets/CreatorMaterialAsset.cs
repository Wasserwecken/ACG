using System.Collections.Generic;
using OpenTK.Mathematics;
using SharpGLTF.Schema2;
using OpenTK.Graphics.OpenGL;
using Framework.Assets.Materials;

namespace Framework.ECS.GLTF2.Assets
{
    public static class CreatorMaterialAsset
    {
        public static MaterialAsset Create(Material gltfMaterial)
        {
            var material = new MaterialAsset(gltfMaterial.Name);
            material.IsTransparent = gltfMaterial.Alpha != AlphaMode.OPAQUE;
            material.CullingMode = gltfMaterial.DoubleSided ? CullFaceMode.FrontAndBack : material.CullingMode;
            AddChannels(material, gltfMaterial.Channels);

            return material;
        }

        /// <summary>
        /// 
        /// </summary>
        private static void AddChannels(MaterialAsset material, IEnumerable<MaterialChannel> channels)
        {
            material.SetUniform("MREO", Vector4.Zero);
            
            foreach (var channel in channels)
            {
                switch (channel.Key)
                {
                    case "BaseColor":
                        material.SetUniform("BaseColor", channel.Parameter.ToOpenTK());
                        break;

                    case "MetallicRoughness":
                        if (material.UniformVecs.TryGetValue("MREO", out var mreo))
                        {
                            mreo.X = channel.Parameter.X;
                            mreo.Y = channel.Parameter.Y;
                            material.SetUniform("MREO", mreo);
                        }

                        break;

                    case "Emissive":
                        if (material.UniformVecs.TryGetValue("MREO", out mreo))
                        {
                            mreo.Z = channel.Parameter.X;
                            material.SetUniform("MREO", mreo);
                        }

                        break;

                    case "Occlusion":
                        if (material.UniformVecs.TryGetValue("MREO", out mreo))
                        {
                            mreo.W = channel.Parameter.X;
                            material.SetUniform("MREO", mreo);
                        }

                        break;

                    case "Normal":
                        material.SetUniform("Normal", channel.Parameter.X);
                        break;
                }
            }
        }
    }
}
