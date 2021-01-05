using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Framework
{
    public class PerspectiveCameraSystem
    {
        public static void Use(WorldTransformComponent transform, PerspectiveCameraComponent camera, ref ViewSpaceData viewSpace)
        {
            GL.ClearColor(camera.ClearColor.X, camera.ClearColor.Y, camera.ClearColor.Z, camera.ClearColor.W);
            GL.Clear(camera.ClearMask);

            var projectionSpace = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(camera.FieldOfView),
                camera.AspectRatio,
                camera.NearClipping,
                camera.FarClipping
            );

            viewSpace.WorldToView = transform.Space;
            viewSpace.WorldToViewRotation = transform.Space.ClearScale().ClearTranslation();
            viewSpace.WorldToProjection = viewSpace.WorldToView * projectionSpace;
        }
    }
}
