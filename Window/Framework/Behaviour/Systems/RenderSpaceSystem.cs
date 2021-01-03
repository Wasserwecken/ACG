using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public static class RenderSpaceSystem
    {
        public static void Update(TransformComponent objectTransform, ViewSpaceData viewSpace, ref RenderSpaceData space)
        {
            space.LocalToWorld = objectTransform.Space;
            space.LocalToWorldRotation = objectTransform.RotationSpace;
            space.LocalToView = space.LocalToWorld * viewSpace.WorldToView;
            space.LocalToViewRotation = objectTransform.RotationSpace * viewSpace.WorldToViewRotation;
            space.LocalToProjection = space.LocalToWorld * viewSpace.WorldToProjection;

            space.WorldToView = viewSpace.WorldToView;
            space.ViewPosition = new Vector4(viewSpace.ViewPosition);
            space.ViewDirection = new Vector4(viewSpace.ViewDirection);
        }
    }
}
