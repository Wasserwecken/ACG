using System.Collections.Generic;
using OpenTK.Mathematics;
using SharpGLTF.Schema2;
using OpenTK.Graphics.OpenGL;
using Framework.Assets.Materials;
using Framework.Assets.Textures;

namespace Framework.ECS.GLTF2.Assets
{
    public static class CreatorMaterialAsset
    {
        public static MaterialAsset Create(Material gltfMaterial, Dictionary<Texture, TextureBaseAsset> textures)
        {
            var material = new MaterialAsset(gltfMaterial.Name);
            material.IsTransparent = gltfMaterial.Alpha != AlphaMode.OPAQUE;
            material.CullingMode = gltfMaterial.DoubleSided ? CullFaceMode.FrontAndBack : material.CullingMode;

            material.SetUniform("MREO", Vector4.Zero);
            foreach (var channel in gltfMaterial.Channels)
            {
                switch (channel.Key)
                {
                    case "BaseColor":
                        material.SetUniform("BaseColor", channel.Parameter.ToOpenTK());
                        if (channel.Texture != null)
                            material.SetUniform("BaseColorMap", textures[channel.Texture]);
                        break;

                    case "MetallicRoughness":
                        if (material.UniformVecs.TryGetValue("MREO", out var mreo))
                        {
                            mreo.X = channel.Parameter.X;
                            mreo.Y = channel.Parameter.Y;
                            material.SetUniform("MREO", mreo);
                        }
                        if (channel.Texture != null)
                            material.SetUniform("MetallicRoughnessMap", textures[channel.Texture]);

                        break;

                    case "Emissive":
                        if (material.UniformVecs.TryGetValue("MREO", out mreo))
                        {
                            mreo.Z = channel.Parameter.X;
                            material.SetUniform("MREO", mreo);
                        }
                        if (channel.Texture != null)
                            material.SetUniform("EmissiveMap", textures[channel.Texture]);

                        break;

                    case "Occlusion":
                        if (material.UniformVecs.TryGetValue("MREO", out mreo))
                        {
                            mreo.W = channel.Parameter.X;
                            material.SetUniform("MREO", mreo);
                        }
                        if (channel.Texture != null)
                            material.SetUniform("OcclusionMap", textures[channel.Texture]);

                        break;

                    case "Normal":
                        material.SetUniform("Normal", channel.Parameter.X);
                        if (channel.Texture != null)
                            material.SetUniform("NormalMap", textures[channel.Texture]);
                        break;
                }
            }

            return material;
        }
    }
}
