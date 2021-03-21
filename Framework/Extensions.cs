using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Framework.ECS;
using OpenTK.Mathematics;

namespace Framework
{
    public static class Extensions
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

        /// <summary>
        /// 
        /// </summary>
        public static Vector3 Rotate(this Vector3 vector, float angle, Vector3 axis)
        {
            return vector * MathF.Cos(angle)
                + Vector3.Cross(vector, axis) * MathF.Sin(angle)
                + Vector3.Dot(vector, axis) * axis * (1 - MathF.Cos(angle));
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool Has<TComponent>(this IEnumerable<IComponent> components, out TComponent result) where TComponent : IComponent
        {
            foreach (var component in components)
            {
                if (component is TComponent)
                {
                    result = (TComponent)component;
                    return true;
                }
            }
            
            result = default;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public static TComponent Get<TComponent>(this IEnumerable<IComponent> components) where TComponent : IComponent
        {
            foreach (var component in components)
                if (component is TComponent resultComponent)
                    return resultComponent;

            return default;
        }
    }
}
