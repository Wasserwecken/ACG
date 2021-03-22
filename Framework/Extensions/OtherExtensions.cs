using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Extensions
{
    public static class OtherExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        public static Vector3 Rotate(this Vector3 vector, float angle, Vector3 axis)
        {
            return vector * MathF.Cos(angle)
                + Vector3.Cross(vector, axis) * MathF.Sin(angle)
                + Vector3.Dot(vector, axis) * axis * (1 - MathF.Cos(angle));
        }
    }
}
