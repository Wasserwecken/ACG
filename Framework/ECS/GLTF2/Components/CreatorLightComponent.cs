using Framework.ECS;
using SharpGLTF.Schema2;
using Framework.ECS.Components.Light;
using OpenTK.Mathematics;

namespace Framework.ECS.GLTF2.Components
{
    public static class CreatorLightComponent
    {
        public static IComponent Create(PunctualLight gltfLight)
        {
            IComponent light = null;

            switch (gltfLight.LightType)
            {
                case PunctualLightType.Directional:
                    light = new DirectionalLightComponent()
                    {
                        Color = new Vector3(gltfLight.Color.X, gltfLight.Color.Y, gltfLight.Color.Z) * gltfLight.Intensity,
                        AmbientFactor = 0.02f,
                    };
                    break;

                case PunctualLightType.Point:
                    light = new PointLightComponent()
                    {
                        Color = new Vector3(gltfLight.Color.X, gltfLight.Color.Y, gltfLight.Color.Z) * gltfLight.Intensity,
                        AmbientFactor = 0.01f,
                    };
                    break;

                case PunctualLightType.Spot:
                    light = new SpotLightComponent()
                    {
                        Color = new Vector3(gltfLight.Color.X, gltfLight.Color.Y, gltfLight.Color.Z) * gltfLight.Intensity,
                        AmbientFactor = 0.01f,
                        OuterAngle = gltfLight.OuterConeAngle,
                        InnerAngle = gltfLight.InnerConeAngle,
                    };
                    break;
            }

            return light;
        }
    }
}
