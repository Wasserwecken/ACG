using System.Collections.Generic;
using OpenTK.Mathematics;
using SharpGLTF.Schema2;
using OpenTK.Graphics.OpenGL;
using Framework.Assets.Materials;
using Framework.Assets.Textures;
using Framework.Extensions;

namespace Framework.ECS.GLTF2.Assets
{
    public static class CreatorMaterialAsset
    {
        public static MaterialAsset Create(Material gltfMaterial, Dictionary<Texture, TextureBaseAsset> textures)
        {
            var material = new MaterialAsset(gltfMaterial.Name);
            material.IsTransparent = gltfMaterial.Alpha != AlphaMode.OPAQUE;
            material.CullingMode = gltfMaterial.DoubleSided ? CullFaceMode.FrontAndBack : material.CullingMode;

            material.SetUniform(Definitions.Shader.Uniform.MREO, Vector4.Zero);
            foreach (var channel in gltfMaterial.Channels)
            {
                switch (channel.Key)
                {
                    case "BaseColor":
                        material.SetUniform(Definitions.Shader.Uniform.BaseColor, channel.Parameter.ToOpenTK());
                        if (channel.Texture != null)
                            material.SetUniform(Definitions.Shader.Uniform.BaseColorMap, textures[channel.Texture]);
                        break;

                    case "MetallicRoughness":
                        if (material.UniformVecs.TryGetValue(Definitions.Shader.Uniform.MREO, out var mreo))
                        {
                            mreo.X = channel.Parameter.X;
                            mreo.Y = channel.Parameter.Y;
                            material.SetUniform(Definitions.Shader.Uniform.MREO, mreo);
                        }
                        if (channel.Texture != null)
                            material.SetUniform(Definitions.Shader.Uniform.MetallicRoughnessMap, textures[channel.Texture]);

                        break;

                    case "Emissive":
                        if (material.UniformVecs.TryGetValue(Definitions.Shader.Uniform.MREO, out mreo))
                        {
                            mreo.Z = channel.Parameter.X;
                            material.SetUniform(Definitions.Shader.Uniform.MREO, mreo);
                        }
                        if (channel.Texture != null)
                            material.SetUniform(Definitions.Shader.Uniform.EmissiveMap, textures[channel.Texture]);

                        break;

                    case "Occlusion":
                        if (material.UniformVecs.TryGetValue(Definitions.Shader.Uniform.MREO, out mreo))
                        {
                            mreo.W = channel.Parameter.X;
                            material.SetUniform(Definitions.Shader.Uniform.MREO, mreo);
                        }
                        if (channel.Texture != null)
                            material.SetUniform(Definitions.Shader.Uniform.OcclusionMap, textures[channel.Texture]);

                        break;

                    case "Normal":
                        material.SetUniform(Definitions.Shader.Uniform.Normal, channel.Parameter.X);
                        if (channel.Texture != null)
                            material.SetUniform(Definitions.Shader.Uniform.NormalMap, textures[channel.Texture]);
                        break;
                }
            }

            return material;
        }
    }
}
