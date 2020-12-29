using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public static class SpaceSystem
    {
        public static void Update(TransformData worldTransformData, TransformData viewTransformData, ref SpaceData spaceData)
        {
            spaceData.LocalToWorld = worldTransformData.Space;
            spaceData.LocalToView = spaceData.LocalToWorld * viewTransformData.Space;
            spaceData.LocalToProjection = spaceData.LocalToView;

            spaceData.LocalToWorldRotation = worldTransformData.RotationSpace;
            spaceData.LocalToViewRotation = worldTransformData.RotationSpace * viewTransformData.RotationSpace;
        }
    }
}
