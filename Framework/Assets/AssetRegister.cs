using Framework;
using Framework.Assets.Framebuffer;
using Framework.Assets.Materials;
using Framework.Assets.Shader;
using Framework.Assets.Textures;
using Framework.Assets.Verticies;
using System.Collections.Generic;

namespace ACG.Framework.Assets
{
    public static class AssetRegister
    {
        public static readonly List<MaterialAsset> Materials;
        public static readonly List<TextureBaseAsset> Textures;
        public static readonly List<ShaderProgramAsset> Shaders;
        public static readonly List<FramebufferAsset> Framebuffers;
        public static readonly List<VertexPrimitiveAsset> Primitives;
        public static readonly List<IShaderBlock> ShaderBlocks;

        /// <summary>
        /// 
        /// </summary>
        static AssetRegister()
        {
            Materials = new List<MaterialAsset>();
            Textures = new List<TextureBaseAsset>();
            Shaders = new List<ShaderProgramAsset>();
            Framebuffers = new List<FramebufferAsset>();
            Primitives = new List<VertexPrimitiveAsset>();
            ShaderBlocks = new List<IShaderBlock>();
        }
    }
}
