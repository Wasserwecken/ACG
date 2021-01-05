using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public static class RenderSpaceSystem
    {
        public static void Update(WorldTransformComponent objectTransform, ViewSpaceData viewSpace, ref ShaderSpaceData space)
        {
            space.LocalToWorld = objectTransform.Space;
            space.LocalToWorldRotation = objectTransform.Space.ClearScale().ClearTranslation();
            space.LocalToView = space.LocalToWorld * viewSpace.WorldToView;
            space.LocalToViewRotation = space.LocalToWorldRotation * viewSpace.WorldToViewRotation;
            space.LocalToProjection = space.LocalToWorld * viewSpace.WorldToProjection;

            space.WorldToView = viewSpace.WorldToView;
            space.ViewPosition = new Vector4(viewSpace.ViewPosition);
            space.ViewDirection = new Vector4(viewSpace.ViewDirection);
        }
    }
}
