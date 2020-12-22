using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public class PerspectiveCamera : CameraBase
    {
        public PerspectiveCameraData PerspectiveData { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public override void Use(ref RenderData renderData)
        {
            base.Use(ref renderData);

            var projectionSpace = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(PerspectiveData.FieldOfView),
                BaseData.AspectRatio, BaseData.NearClipping, BaseData.FarClipping
            );

            renderData.WorldToProjection = BaseData.Transform.Space * projectionSpace;
        }
    }
}
