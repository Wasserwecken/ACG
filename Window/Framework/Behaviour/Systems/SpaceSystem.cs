using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public static class SpaceSystem
    {
        public static void Update(TransformData worldTransform, TransformData viewTransform, ref SpaceData space)
        {
            space.LocalToWorld = worldTransform.Space;
            space.LocalToView = space.LocalToWorld * viewTransform.Space;

            space.LocalToWorldRotation = worldTransform.RotationSpace;
            space.LocalToViewRotation = worldTransform.RotationSpace * viewTransform.RotationSpace;
        }
    }
}
