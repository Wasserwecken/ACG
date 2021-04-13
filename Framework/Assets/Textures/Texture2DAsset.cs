using OpenTK.Graphics.OpenGL;

namespace Framework.Assets.Textures
{
    public class Texture2DAsset : TextureBaseAsset
    {
        public override TextureTarget Target => TextureTarget.Texture2D;
        public override GenerateMipmapTarget MipMapTarget => GenerateMipmapTarget.Texture2D;
        public ImageAsset Image
        {
            get => _image;
            set
            {
                _image = value;
                SetPixelInfos(_image.Image);
            }
        }

        private ImageAsset _image;


        /// <summary>
        /// 
        /// </summary>
        public Texture2DAsset(string name) : base(name) { }
    }
}
