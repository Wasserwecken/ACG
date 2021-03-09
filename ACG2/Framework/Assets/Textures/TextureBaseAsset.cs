using OpenTK.Graphics.OpenGL;

namespace Framework.Assets.Textures
{
    public abstract class TextureBaseAsset
    {
        public int Handle { get; set; }
        public string Name { get; set; }
        public abstract TextureTarget Target { get; }
        public abstract GenerateMipmapTarget MipMapTarget { get; }
        public TextureWrapMode WrapModeS { get; set; }
        public TextureWrapMode WrapModeT { get; set; }
        public TextureMinFilter MinFilter { get; set; }
        public TextureMagFilter MagFilter { get; set; }
        public bool GenerateMipMaps { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TextureBaseAsset(string name)
        {
            Name = name;
            GenerateMipMaps = true;

            WrapModeS = TextureWrapMode.Repeat;
            WrapModeT = TextureWrapMode.Repeat;
            MinFilter = TextureMinFilter.Linear;
            MagFilter = TextureMagFilter.Linear;
        }
    }
}
