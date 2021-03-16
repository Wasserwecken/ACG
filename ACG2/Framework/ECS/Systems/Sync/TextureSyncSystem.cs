﻿using Framework.Assets.Textures;
using Framework.ECS.Components.Scene;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using Framework.ECS.Components.Render;

namespace Framework.ECS.Systems.Sync
{
    public class TextureSyncSystem : ISystem
    {
        /// <summary>
        /// 
        /// </summary>
        public TextureSyncSystem()
        {
            if (Default.Texture.White.Handle <= 0) PushTexture(Default.Texture.White);
            if (Default.Texture.Gray.Handle <= 0) PushTexture(Default.Texture.Gray);
            if (Default.Texture.Black.Handle <= 0) PushTexture(Default.Texture.Black);
            if (Default.Texture.Normal.Handle <= 0) PushTexture(Default.Texture.Normal);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Run(IEnumerable<Entity> entities, IEnumerable<IComponent> sceneComponents)
        {
            var renderDataComponent = sceneComponents.First(f => f is RenderDataComponent) as RenderDataComponent;
            var textures = new HashSet<TextureBaseAsset>();

            foreach (var material in renderDataComponent.Materials)
                foreach (var textureUniform in material.UniformTextures)
                    textures.Add(textureUniform.Value);

            foreach (var skyboxComponent in sceneComponents.Where(f => f is SkyboxComponent))
                foreach (var textureUniform in ((SkyboxComponent)skyboxComponent).Material.UniformTextures)
                    textures.Add(textureUniform.Value);

            foreach (var texture in textures)
                if (texture.Handle <= 0)
                    PushTexture(texture);
        }

        /// <summary>
        /// 
        /// </summary>
        private void PushTexture(TextureBaseAsset texture)
        {
            texture.Handle = GL.GenTexture();
            GL.BindTexture(texture.Target, texture.Handle);

            if (texture is Texture2DAsset texture2D)
                SpecificTexture2D(texture2D);
            else if (texture is TextureCubeAsset textureCube)
                SpecificTextureCube(textureCube);

            GL.TexParameter(texture.Target, TextureParameterName.TextureWrapS, (int)texture.WrapModeS);
            GL.TexParameter(texture.Target, TextureParameterName.TextureWrapT, (int)texture.WrapModeT);
            GL.TexParameter(texture.Target, TextureParameterName.TextureMinFilter, (int)texture.MinFilter);
            GL.TexParameter(texture.Target, TextureParameterName.TextureMagFilter, (int)texture.MagFilter);

            if (texture.GenerateMipMaps)
                GL.GenerateMipmap(texture.MipMapTarget);

            GL.BindTexture(texture.Target, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        private void SpecificTexture2D(Texture2DAsset texture)
        {
            GL.TexImage2D(
                texture.Target,
                0,
                texture.Image.InternalFormat,
                texture.Image.Width,
                texture.Image.Height,
                0,
                texture.Image.Format,
                texture.Image.PixelType,
                texture.Image.Data
            );
        }

        /// <summary>
        /// 
        /// </summary>
        private void SpecificTextureCube(TextureCubeAsset texture)
        {
            for (int i = 0; i < 6; i++)
                GL.TexImage2D(
                    TextureTarget.TextureCubeMapPositiveX + i,
                    0,
                    texture.Images[i].InternalFormat,
                    texture.Images[i].Width,
                    texture.Images[i].Height,
                    0,
                    texture.Images[i].Format,
                    texture.Images[i].PixelType,
                    texture.Images[i].Data
                );

            GL.TexParameter(texture.Target, TextureParameterName.TextureWrapR, (int)texture.WrapModeR);
        }
    }
}
