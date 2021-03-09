﻿using System;
using System.Runtime.InteropServices;
using OpenTK.Mathematics;

namespace Framework
{
    public static class Extensions
    {        
        /// <summary>
        /// 
        /// </summary>
        public static byte[] ToBytes<TType>(this TType[] data) where TType : struct
        {
            var typeSize = Marshal.SizeOf<TType>();
            var result = new byte[data.Length * typeSize];
            Buffer.BlockCopy(data, 0, result, 0, result.Length);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public static TType[] ToType<TType>(this byte[] data) where TType : struct
        {
            var typeSize = Marshal.SizeOf<TType>();
            var result = new TType[data.Length / typeSize];
            Buffer.BlockCopy(data, 0, result, 0, data.Length);

            return result;
        }

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