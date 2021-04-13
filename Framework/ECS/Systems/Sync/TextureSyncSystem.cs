using Framework.Assets.Textures;
using OpenTK.Graphics.OpenGL;
using Framework.ECS.Components.Render;
using DefaultEcs.System;
using DefaultEcs;
using ImageMagick;
using ACG.Framework.Assets;

namespace Framework.ECS.Systems.Sync
{
    public class TextureSyncSystem : AComponentSystem<bool, PrimitiveComponent>
    {
        private readonly Entity _worldComponents;

        /// <summary>
        /// 
        /// </summary>
        public TextureSyncSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void PreUpdate(bool state)
        {
            foreach(var texture in AssetRegister.Textures)
                if (texture.Handle <= 0)
                {
                    texture.Handle = GL.GenTexture();
                    GL.BindTexture(texture.Target, texture.Handle);

                    if (texture is Texture2DAsset texture2D)
                        SpecificTexture2D(texture2D);
                    else if (texture is TextureCubeAsset textureCube)
                        SpecificTextureCube(textureCube);
                    else if (texture is TextureRenderAsset textureRender)
                        SpecificTextureRender(textureRender);

                    GL.TexParameter(texture.Target, TextureParameterName.TextureWrapS, (int)texture.WrapModeS);
                    GL.TexParameter(texture.Target, TextureParameterName.TextureWrapT, (int)texture.WrapModeT);
                    GL.TexParameter(texture.Target, TextureParameterName.TextureMinFilter, (int)texture.MinFilter);
                    GL.TexParameter(texture.Target, TextureParameterName.TextureMagFilter, (int)texture.MagFilter);

                    if (texture.GenerateMipMaps)
                        GL.GenerateMipmap(texture.MipMapTarget);

                    GL.BindTexture(texture.Target, 0);
                }    
        }

        /// <summary>
        /// 
        /// </summary>
        private void SpecificTexture2D(Texture2DAsset texture)
        {
            if (texture.Image.Image != null)
                GetPixelInfos(texture.Image.Image, out texture.PixelType, out texture.Format, out texture.InternalFormat);

            GL.TexImage2D(
                texture.Target,
                0,
                texture.InternalFormat,
                texture.Image.Width,
                texture.Image.Height,
                0,
                texture.Format,
                texture.PixelType,
                texture.Image.Data
            );
        }

        /// <summary>
        /// 
        /// </summary>
        private void SpecificTextureCube(TextureCubeAsset texture)
        {
            if (texture.Images[0].Image != null)
                GetPixelInfos(texture.Images[0].Image, out texture.PixelType, out texture.Format, out texture.InternalFormat);
            
            for (int i = 0; i < 6; i++)
                GL.TexImage2D(
                    TextureTarget.TextureCubeMapPositiveX + i,
                    0,
                    texture.InternalFormat,
                    texture.Images[i].Width,
                    texture.Images[i].Height,
                    0,
                    texture.Format,
                    texture.PixelType,
                    texture.Images[i].Data
                );

            GL.TexParameter(texture.Target, TextureParameterName.TextureWrapR, (int)texture.WrapModeR);
        }

        /// <summary>
        /// 
        /// </summary>
        private void SpecificTextureRender(TextureRenderAsset texture)
        {
            GL.TexImage2D(
                texture.Target,
                0,
                texture.InternalFormat,
                texture.Width,
                texture.Height,
                0,
                texture.Format,
                texture.PixelType,
                default
            );
        }

        /// <summary>
        /// 
        /// </summary>
        protected void GetPixelInfos(MagickImage image, out PixelType type, out PixelFormat format, out PixelInternalFormat internalFormat)
        {
            switch (image.ChannelCount)
            {
                case 1:
                    format = PixelFormat.Red;
                    break;
                case 2:
                    format = PixelFormat.Rg;
                    break;
                case 3:
                    format = PixelFormat.Rgb;
                    break;
                case 4:
                    format = PixelFormat.Rgba;
                    break;
                default:
                    format = default;
                    break;
            }

            type = PixelType.UnsignedShort;
            internalFormat = (PixelInternalFormat)format;
        }
    }
}
