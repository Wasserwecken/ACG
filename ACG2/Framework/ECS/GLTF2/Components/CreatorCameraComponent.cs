using SharpGLTF.Schema2;
using OpenTK.Mathematics;
using Framework.ECS.Components.Render;
using OpenTK.Graphics.OpenGL;

namespace Framework.ECS.GLTF2.Components
{
    public static class CreatorCameraComponent
    {
        public static IComponent Create(Camera gltfCamera)
        {
            IComponent camera = null;

            if (gltfCamera.Settings.IsPerspective)
            {
                var glTFperspective = (CameraPerspective)gltfCamera.Settings;
                camera = new PerspectiveCameraComponent()
                {
                    NearClipping = glTFperspective.ZNear,
                    FarClipping = glTFperspective.ZFar,
                    FieldOfView = MathHelper.RadiansToDegrees(glTFperspective.VerticalFOV),
                    ClearMask = ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit,
                    ClearColor = new Vector4(0.3f)
                };
            }

            return camera;
        }
    }
}
