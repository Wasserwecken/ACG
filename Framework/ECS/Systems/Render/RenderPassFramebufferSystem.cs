using DefaultEcs;
using DefaultEcs.System;
using Framework.ECS.Components.Render;
using OpenTK.Graphics.OpenGL;

namespace Framework.ECS.Systems.Render
{
    public class RenderPassFramebufferSystem : AEntitySetSystem<bool>
    {
        private readonly Entity _worldComponents;

        /// <summary>
        /// 
        /// </summary>
        public RenderPassFramebufferSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update(bool state, in Entity entity)
        {
            var frameBuffer = entity.Get<RenderPassFrameBufferComponent>();

            foreach(var texture in frameBuffer.FrameBuffer.TextureTargets)
            {
                if (texture.Handle <= 0)
                {
                    texture.Handle = GL.GenTexture();
                    GL.BindTexture(texture.Target, texture.Handle);

                    GL.TexImage2D(
                        texture.Target,
                        0,
                        texture.InternalFormat,
                        frameBuffer.FrameBuffer.Width,
                        frameBuffer.FrameBuffer.Height,
                        0,
                        texture.Format,
                        texture.PixelType,
                        default
                    );

                    GL.TexParameter(texture.Target, TextureParameterName.TextureWrapS, (int)texture.WrapModeS);
                    GL.TexParameter(texture.Target, TextureParameterName.TextureWrapT, (int)texture.WrapModeT);
                    GL.TexParameter(texture.Target, TextureParameterName.TextureMinFilter, (int)texture.MinFilter);
                    GL.TexParameter(texture.Target, TextureParameterName.TextureMagFilter, (int)texture.MagFilter);

                    if (texture.GenerateMipMaps)
                        GL.GenerateMipmap(texture.MipMapTarget);

                    GL.BindTexture(texture.Target, 0);
                }
            }

            if (frameBuffer.FrameBuffer.Handle <= 0)
            {
                frameBuffer.FrameBuffer.Handle = GL.GenFramebuffer();
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer.FrameBuffer.Handle);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, 2343543, 0);
                GL.DrawBuffer(DrawBufferMode.None);
                GL.ReadBuffer(ReadBufferMode.None);
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            }
        }
    }
}
