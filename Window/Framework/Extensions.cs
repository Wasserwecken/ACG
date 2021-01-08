using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Framework
{
    public static class Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        public static int IndexOf<TType>(this TType[] array, Func<TType, bool> comparer)
        {
            for (int i = 0; i < array.Length; i++)
                if (comparer(array[i])) return i;

            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        public static Matrix4 ToOpenTK(this System.Numerics.Matrix4x4 matrix)
        {
            return new Matrix4(
                matrix.M11, matrix.M12, matrix.M13, matrix.M14,
                matrix.M21, matrix.M22, matrix.M23, matrix.M24,
                matrix.M31, matrix.M32, matrix.M33, matrix.M34,
                matrix.M41, matrix.M42, matrix.M43, matrix.M44
            );
        }

        /// <summary>
        /// 
        /// </summary>
        public static Vector4 ToOpenTK(this System.Numerics.Vector4 vector)
        {
            return new Vector4(
                vector.X,
                vector.Y,
                vector.Z,
                vector.W
            );
        }
    }
}
