using Framework.Assets.Framebuffer;
using Framework.Assets.Shader;
using Framework.Assets.Textures;
using Framework.Assets.Verticies;
using Framework.Assets.Verticies.Attributes;
using Framework.Extensions;
using ImageMagick;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Linq;

namespace Framework.ECS.Systems.Render.OpenGL
{
    public static class GPUSync
    {
        /// <summary>
        /// 
        /// </summary>
        public static void Push(ShaderBlockBase shaderBlock)
        {
            if (shaderBlock.Handle < 0)
                shaderBlock.Handle = GL.GenBuffer();

            var bytes = shaderBlock.GetBytes();

            GL.BindBuffer((BufferTarget)shaderBlock.Target, shaderBlock.Handle);
            GL.BufferData((BufferTarget)shaderBlock.Target, bytes.Length, bytes, shaderBlock.UsageHint);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Push(FramebufferAsset framebuffer)
        {
            framebuffer.Handle = GL.GenFramebuffer();
            GL.BindFramebuffer(framebuffer.Target, framebuffer.Handle);

            foreach(var storage in framebuffer.Storages)
            {
                if (storage.Handle <= 0) Push(storage);
                GL.FramebufferRenderbuffer(framebuffer.Target, storage.Attachment, storage.Target, storage.Handle);
            }

            foreach (var texture in framebuffer.Textures)
            {
                if (texture.Handle <= 0) Push(texture);
                GL.FramebufferTexture2D(framebuffer.Target, texture.Attachment, texture.Target, texture.Handle, 0);
            }

            GL.DrawBuffer(framebuffer.DrawMode);
            GL.ReadBuffer(framebuffer.ReadMode);
            GL.DrawBuffers(framebuffer.DrawTargets.Length, framebuffer.DrawTargets);

            GL.BindFramebuffer(framebuffer.Target, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Push(FramebufferStorageAsset storage)
        {
            storage.Handle = GL.GenRenderbuffer();
            GL.RenderbufferStorage(storage.Target, storage.DataType, storage.Width, storage.Height);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Push(TextureBaseAsset texture)
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

            if (texture.AnisotropicFilter > 1f)
                GL.TexParameter(texture.Target, (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, texture.AnisotropicFilter);

            if (texture.GenerateMipMaps)
                GL.GenerateMipmap(texture.MipMapTarget);

            GL.BindTexture(texture.Target, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Push(VertexPrimitiveAsset primitive)
        {
            // creating buffer bytes
            //if (!primitive.ArrayBuffer.Attributes.Any(f => f.Name == Definitions.Shader.Attribute.Tangent.Name))
            CreateTangets(primitive);

            var arrayBuffer = CreateBufferArrayData(primitive.ArrayBuffer);
            var indicieBuffer = CreateBufferIndicieData(primitive.IndicieBuffer);

            // GPU buffer reservation
            primitive.Handle = GL.GenVertexArray();
            primitive.ArrayBuffer.Handle = GL.GenBuffer();

            // send arraybuffer to GPU
            GL.BindVertexArray(primitive.Handle);
            GL.BindBuffer(primitive.ArrayBuffer.Target, primitive.ArrayBuffer.Handle);
            GL.BufferData(primitive.ArrayBuffer.Target, arrayBuffer.Length, arrayBuffer, primitive.ArrayBuffer.UsageHint);

            // send attribute infos to GPU
            var offset = 0;
            foreach (var attribute in primitive.ArrayBuffer.Attributes)
            {
                GL.VertexAttribPointer(attribute.Layout, attribute.Dimension, attribute.PointerType, attribute.Normalize, primitive.ArrayBuffer.ElementSize, offset);
                GL.EnableVertexAttribArray(attribute.Layout);
                offset += attribute.ElementSize;
            }

            // send indicie info to GPU
            if (primitive.IndicieBuffer != null)
            {
                primitive.IndicieBuffer.Handle = GL.GenBuffer();
                GL.BindBuffer(primitive.IndicieBuffer.Target, primitive.IndicieBuffer.Handle);
                GL.BufferData(primitive.IndicieBuffer.Target, indicieBuffer.Length, indicieBuffer, primitive.IndicieBuffer.UsageHint);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static void SpecificTexture2D(Texture2DAsset texture)
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
        private static void SpecificTextureCube(TextureCubeAsset texture)
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
        private static void SpecificTextureRender(TextureRenderAsset texture)
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
        private static void GetPixelInfos(MagickImage image, out PixelType type, out PixelFormat format, out PixelInternalFormat internalFormat)
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

        /// <summary>
        /// 
        /// </summary>
        private static void CreateTangets(VertexPrimitiveAsset primitive)
        {
            var positionAttribute = primitive.ArrayBuffer.Attributes.First(f => f.Name == Definitions.Shader.Attribute.Position.Name) as VertexAttributeVector3;
            var uvAttribute = primitive.ArrayBuffer.Attributes.First(f => f.Name == Definitions.Shader.Attribute.UV.Name) as VertexAttributeVector2;
            var tangentAttribute = new VertexAttributeVector4(
                Definitions.Shader.Attribute.Tangent.Name,
                Definitions.Shader.Attribute.Tangent.Layout,
                Definitions.Shader.Attribute.Tangent.Normalize)
            { DataTyped = new Vector4[positionAttribute.ElementCount] };

            for (int i = 0; i < primitive.IndicieBuffer.Indicies.Length; i += 3)
            {
                var i1 = primitive.IndicieBuffer.Indicies[i + 0];
                var i2 = primitive.IndicieBuffer.Indicies[i + 1];
                var i3 = primitive.IndicieBuffer.Indicies[i + 2];
                var p1 = positionAttribute.DataTyped[i1];
                var p2 = positionAttribute.DataTyped[i2];
                var p3 = positionAttribute.DataTyped[i3];
                var uv1 = uvAttribute.DataTyped[i1];
                var uv2 = uvAttribute.DataTyped[i2];
                var uv3 = uvAttribute.DataTyped[i3];

                var edge1 = p2 - p1;
                var edge2 = p3 - p1;
                var uvDelta1 = uv2 - uv1;
                var uvDelta2 = uv3 - uv1;

                float f = 1.0f / (uvDelta1.X * uvDelta2.Y - uvDelta2.X * uvDelta1.Y);
                var tangent = new Vector4(new Vector3(
                        f * (uvDelta2.Y * edge1.X - uvDelta1.Y * edge2.X),
                        f * (uvDelta2.Y * edge1.Y - uvDelta1.Y * edge2.Y),
                        f * (uvDelta2.Y * edge1.Z - uvDelta1.Y * edge2.Z)
                    ).Normalized(), 1
                );

                tangentAttribute.DataTyped[i1] = tangent;
                tangentAttribute.DataTyped[i2] = tangent;
                tangentAttribute.DataTyped[i3] = tangent;
            }

            primitive.ArrayBuffer.Attributes.Add(tangentAttribute);
        }

        /// <summary>
        /// 
        /// </summary>
        private static byte[] CreateBufferArrayData(BufferArrayAsset buffer)
        {
            // set buffer size
            var result = new byte[buffer.Attributes[0].ElementCount * buffer.ElementSize];

            // prepare
            foreach (var attribute in buffer.Attributes)
                attribute.UpdateByteData();

            // go through each data element
            for (int i = 0; i < buffer.ElementCount; i++)
            {
                var bufferIndex = i * buffer.ElementSize;

                // copy atribute data for a single element into the buffer object
                foreach (var attribute in buffer.Attributes)
                {
                    var attributeIndex = i * attribute.ElementSize;
                    System.Buffer.BlockCopy(attribute.DataBytes, attributeIndex, result, bufferIndex, attribute.ElementSize);
                    bufferIndex += attribute.ElementSize;
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        private static byte[] CreateBufferIndicieData(BufferIndicieAsset buffer)
        {
            return buffer.Indicies.ToBytes();
        }
    }
}
