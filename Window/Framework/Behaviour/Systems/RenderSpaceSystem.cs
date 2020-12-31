﻿using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public static class RenderSpaceSystem
    {
        public static void Update(TransformData objectTransform, ViewSpaceData viewSpace, ref RenderSpaceData space)
        {
            space.LocalToWorld = objectTransform.Space;
            space.LocalToWorldRotation = objectTransform.RotationSpace;

            space.LocalToView = space.LocalToWorld * viewSpace.WorldToView;
            space.LocalToViewRotation = objectTransform.RotationSpace * viewSpace.WorldToViewRotation;

            space.LocalToProjection = space.LocalToWorld * viewSpace.WorldToProjection;
        }
    }
}
