using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public class PerspectiveCameraComponent
    {
        public TransformComponent Transform { get; set; }

        public float FieldOfView { get; set; }
        public float NearClipping { get; set; }
        public float FarClipping { get; set; }
        public float AspectRatio { get; set; }

        public Matrix4 ViewSpace => Transform.Space;
        public Matrix4 ProjectionSpace { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        public void Use()
        {
            ProjectionSpace = ViewSpace * Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(FieldOfView), AspectRatio, NearClipping, FarClipping);
        }
    }
}
