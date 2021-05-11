using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Framework.Extensions
{
    public static class BinaryWriterExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        public static void Write(this BinaryWriter writer, Vector2 vector)
        {
            writer.Write(vector.X);
            writer.Write(vector.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Write(this BinaryWriter writer, Vector3 vector)
        {
            writer.Write(vector.X);
            writer.Write(vector.Y);
            writer.Write(vector.Z);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Write(this BinaryWriter writer, Vector4 vector)
        {
            writer.Write(vector.X);
            writer.Write(vector.Y);
            writer.Write(vector.Z);
            writer.Write(vector.W);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Write(this BinaryWriter writer, Matrix4 matrix)
        {
            writer.Write(matrix.Row0);
            writer.Write(matrix.Row1);
            writer.Write(matrix.Row2);
            writer.Write(matrix.Row3);
        }
    }
}
