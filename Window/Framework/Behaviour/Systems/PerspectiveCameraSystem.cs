using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Framework
{
    public class PerspectiveCameraSystem
    {
        public static void Use(TransformData transform, PerspectiveCameraData camera, ref ViewSpaceData viewSpace)
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Clear(camera.ClearMask);

            var projectionSpace = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(camera.FieldOfView),
                camera.AspectRatio,
                camera.NearClipping,
                camera.FarClipping
            );

            viewSpace.WorldToView = transform.Space;
            viewSpace.WorldToViewRotation = transform.RotationSpace;
            viewSpace.WorldToProjection = viewSpace.WorldToView * projectionSpace;
        }
    }
}
