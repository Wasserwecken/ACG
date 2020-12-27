using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Framework
{
    public class PerspectiveCameraSystem
    {
        public static void Use(PerspectiveCameraData cameraData, ref RenderData renderData)
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Clear(cameraData.ClearMask);

            var projectionSpace = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(cameraData.FieldOfView),
                cameraData.AspectRatio,
                cameraData.NearClipping,
                cameraData.FarClipping
            );

            renderData.ViewPosition = cameraData.Transform.Position;
            renderData.WorldToProjectionSpace = cameraData.Transform.Space * projectionSpace;
            renderData.WorldToViewSpace = cameraData.Transform.Space;
            renderData.WorldToViewRotationSpace = cameraData.Transform.RotationSpace;
        }
    }
}
