using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Framework
{
    public class PerspectiveCameraSystem
    {
        public static void Use(TransformData transform, PerspectiveCameraData camera, ref RenderData render)
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Clear(camera.ClearMask);

            var projectionSpace = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(camera.FieldOfView),
                camera.AspectRatio,
                camera.NearClipping,
                camera.FarClipping
            );


            render.SpaceBlock.Data.LocalToWorld = transform.Space;
            render.SpaceBlock.Data.LocalToView = transform.Space;
            render.SpaceBlock.Data.LocalToProjection = transform.Space * projectionSpace;

            //render.SpaceBlock.Data.LocalToWorldRotation = worldTransformData.RotationSpace;
            //render.SpaceBlock.Data.LocalToViewRotation = worldTransformData.RotationSpace * viewTransformData.RotationSpace;
        }
    }
}
