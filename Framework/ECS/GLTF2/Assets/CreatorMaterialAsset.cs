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

            material.SetUniform(Default.Shader.Uniform.MREO, Vector4.Zero);
            foreach (var channel in gltfMaterial.Channels)
            {
                switch (channel.Key)
                {
                    case "BaseColor":
                        material.SetUniform(Default.Shader.Uniform.BaseColor, channel.Parameter.ToOpenTK());
                        if (channel.Texture != null)
                            material.SetUniform(Default.Shader.Uniform.BaseColorMap, textures[channel.Texture]);
                        break;

                    case "MetallicRoughness":
                        if (material.UniformVecs.TryGetValue(Default.Shader.Uniform.MREO, out var mreo))
                        {
                            mreo.X = channel.Parameter.X;
                            mreo.Y = channel.Parameter.Y;
                            material.SetUniform(Default.Shader.Uniform.MREO, mreo);
                        }
                        if (channel.Texture != null)
                            material.SetUniform(Default.Shader.Uniform.MetallicRoughnessMap, textures[channel.Texture]);

                        break;

                    case "Emissive":
                        if (material.UniformVecs.TryGetValue(Default.Shader.Uniform.MREO, out mreo))
                        {
                            mreo.Z = channel.Parameter.X;
                            material.SetUniform(Default.Shader.Uniform.MREO, mreo);
                        }
                        if (channel.Texture != null)
                            material.SetUniform(Default.Shader.Uniform.EmissiveMap, textures[channel.Texture]);

                        break;

                    case "Occlusion":
                        if (material.UniformVecs.TryGetValue(Default.Shader.Uniform.MREO, out mreo))
                        {
                            mreo.W = channel.Parameter.X;
                            material.SetUniform(Default.Shader.Uniform.MREO, mreo);
                        }
                        if (channel.Texture != null)
                            material.SetUniform(Default.Shader.Uniform.OcclusionMap, textures[channel.Texture]);

                        break;

                    case "Normal":
                        material.SetUniform(Default.Shader.Uniform.Normal, channel.Parameter.X);
                        if (channel.Texture != null)
                            material.SetUniform(Default.Shader.Uniform.NormalMap, textures[channel.Texture]);
                        break;
                }
            }

            return material;
        }
    }
}
