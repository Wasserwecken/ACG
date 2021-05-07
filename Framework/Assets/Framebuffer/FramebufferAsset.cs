using ACG.Framework.Assets;
using Framework.Assets.Textures;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace Framework.Assets.Framebuffer
{
    public class FramebufferAsset
    {
        public string Name { get; }
        public int Handle { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public List<TextureRenderAsset> Textures { get; set; }
        public List<FramebufferStorageAsset> Storages { get; set; }
        public FramebufferTarget Target { get; set; }
        public DrawBufferMode DrawMode { get; set; }
        public ReadBufferMode ReadMode { get; set; }

        public Color4 ClearColor { get; set; }
        public ClearBufferMask ClearMask { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public FramebufferAsset(string name)
        {
            Name = name;
            AssetRegister.Framebuffers.Add(this);

            Target = FramebufferTarget.Framebuffer;
            DrawMode = DrawBufferMode.Front;
            ReadMode = ReadBufferMode.Front;

            ClearColor = Color4.Black;
            ClearMask = ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit;

            Textures = new List<TextureRenderAsset>();
            Storages = new List<FramebufferStorageAsset>();
        }
    }
}
