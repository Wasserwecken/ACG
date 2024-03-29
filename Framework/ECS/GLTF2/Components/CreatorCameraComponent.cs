﻿using SharpGLTF.Schema2;
using OpenTK.Mathematics;
using Framework.ECS.Components.Render;
using OpenTK.Graphics.OpenGL;

namespace Framework.ECS.GLTF2.Components
{
    public static class CreatorCameraComponent
    {
        public static object Create(Camera gltfCamera)
        {
            object camera = null;

            if (gltfCamera.Settings.IsPerspective)
            {
                var glTFperspective = (CameraPerspective)gltfCamera.Settings;
                camera = new PerspectiveCameraComponent()
                {
                    NearClipping = glTFperspective.ZNear,
                    FarClipping = glTFperspective.ZFar,
                    FieldOfView = MathHelper.RadiansToDegrees(glTFperspective.VerticalFOV),
                };
            }

            return camera;
        }
    }
}
