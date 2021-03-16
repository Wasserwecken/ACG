using OpenTK.Mathematics;
using System.Diagnostics;

namespace Framework.ECS.Components.Light
{
    [DebuggerDisplay("Color: {Color}, AmbientFactor: {AmbientFactor}, OuterAngle: {OuterAngle}, InnerAngle: {InnerAngle}")]
    public class SpotLightComponent : IComponent
    {
        public Vector3 Color;
        public float AmbientFactor;
        public float OuterAngle;
        public float InnerAngle;
    }
}
