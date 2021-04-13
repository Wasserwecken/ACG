using ACG.Framework.Assets;
using Framework.Assets.Textures;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace Framework.Assets.Framebuffer
{
    public class FramebufferAsset
    {
        public int Handle { get; set; }
        public int Width { get; }
        public int Height { get; }
        public bool HasColorOutput { get; set; }

        public List<TextureRenderAsset> TextureTargets { get; }
        public List<FramebufferStorageAsset> StorageTargets { get; }
        public FramebufferTarget Target { get; }
        public Color4 ClearColor { get; set; }
        public ClearBufferMask ClearMask { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public FramebufferAsset(int width, int height)
        {
            AssetRegister.Framebuffers.Add(this);

            Width = width;
            Height = height;

            Target = FramebufferTarget.Framebuffer;
            ClearColor = Color4.Black;
            ClearMask = ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit;

            TextureTargets = new List<TextureRenderAsset>();
            StorageTargets = new List<FramebufferStorageAsset>();
        }
    }
}
