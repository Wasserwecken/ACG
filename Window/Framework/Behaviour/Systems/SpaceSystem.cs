using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public static class SpaceSystem
    {
        public static void Update(TransformData objectTransform, ViewSpaceData viewSpace, ref RenderSpaceData space)
        {
            space.LocalToWorld = objectTransform.Space;
            space.LocalToWorldRotation = objectTransform.RotationSpace;

            space.LocalToView = space.LocalToWorld * viewSpace.WorldToView;
            space.LocalToViewRotation = space.LocalToWorldRotation * viewSpace.WorldToViewRotation;

            space.LocalToProjection = space.LocalToWorld * viewSpace.WorldToProjection;
        }
    }
}
