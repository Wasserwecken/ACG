using Framework.Assets.Textures;

namespace Framework.ECS.Components.Light
{
    public struct ReflectionProbeComponent
    {
        public int InfoID;
        public bool HasChanged;
        public int Resolution;
        public float NearClipping;
        public float FarClipping;
        public TextureCubeAsset Skybox;
    }
}
