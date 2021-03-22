using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Framework.Extensions
{
    public static class ConversionExtensions
    {   
        /// <summary>
        /// 
        /// </summary>
        public static ushort[] ToShort(this Color4 color)
        {
            return new ushort[]
            {
                (ushort)(color.R * ushort.MaxValue),
                (ushort)(color.G * ushort.MaxValue),
                (ushort)(color.B * ushort.MaxValue),
                (ushort)(color.A * ushort.MaxValue)
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public static byte[] ToBytes(this Vector4 data)
        {
            var result = new List<byte>();

            result.AddRange(BitConverter.GetBytes(data.X));
            result.AddRange(BitConverter.GetBytes(data.Y));
            result.AddRange(BitConverter.GetBytes(data.Z));
            result.AddRange(BitConverter.GetBytes(data.W));

            return result.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        public static byte[] ToBytes(this Vector3 data)
        {
            var result = new List<byte>();

            result.AddRange(BitConverter.GetBytes(data.X));
            result.AddRange(BitConverter.GetBytes(data.Y));
            result.AddRange(BitConverter.GetBytes(data.Z));

            return result.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        public static byte[] ToBytes(this Vector2 data)
        {
            var result = new List<byte>();

            result.AddRange(BitConverter.GetBytes(data.X));
            result.AddRange(BitConverter.GetBytes(data.Y));

            return result.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        public static byte[] ToBytes(this uint[] data)
        {
            var result = new List<byte>();

            for (int i = 0; i < data.Length; i++)
                result.AddRange(BitConverter.GetBytes(data[i]));

            return result.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        public static Vector4[] ToOpenTK(this System.Numerics.Vector4[] source)
        {
            var result = new Vector4[source.Length];

            for (int i = 0; i < result.Length; i++)
                result[i] = source[i].ToOpenTK();

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public static Vector3[] ToOpenTK(this System.Numerics.Vector3[] source)
        {
            var result = new Vector3[source.Length];

            for (int i = 0; i < result.Length; i++)
                result[i] = source[i].ToOpenTK();

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public static Vector2[] ToOpenTK(this System.Numerics.Vector2[] source)
        {
            var result = new Vector2[source.Length];

            for (int i = 0; i < result.Length; i++)
                result[i] = source[i].ToOpenTK();

            return result;
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

        /// <summary>
        /// 
        /// </summary>
        public static Vector3 ToOpenTK(this System.Numerics.Vector3 vector)
        {
            return new Vector3(
                vector.X,
                vector.Y,
                vector.Z
            );
        }

        /// <summary>
        /// 
        /// </summary>
        public static Vector2 ToOpenTK(this System.Numerics.Vector2 vector)
        {
            return new Vector2(
                vector.X,
                vector.Y
            );
        }

    }
}
