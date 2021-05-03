using ACG.Framework.Assets;
using DefaultEcs;
using DefaultEcs.System;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace ACG.Framework.ECS.Systems.Sync
{
    public class FramebufferSyncSystem : AEntitySetSystem<bool>
    {
        private readonly Entity _worldComponents;

        /// <summary>
        /// 
        /// </summary>
        public FramebufferSyncSystem(World world, Entity worldComponents) : base(world)
        {
            _worldComponents = worldComponents;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void PreUpdate(bool state)
        {
            foreach(var framebuffer in AssetRegister.Framebuffers)
                if (framebuffer.Handle <= 0)
                {
                    framebuffer.Handle = GL.GenFramebuffer();
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer.Handle);

                    foreach(var texture in framebuffer.TextureTargets)
                        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, texture.Attachment, texture.Target, texture.Handle, 0);

                    GL.DrawBuffer(DrawBufferMode.None);
                    GL.ReadBuffer(ReadBufferMode.None);

                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                }
        }
    }
}
