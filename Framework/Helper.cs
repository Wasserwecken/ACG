using Framework.Assets.Textures;
using ImageMagick;
using System.IO;
using OpenTK.Mathematics;
using System;

namespace Framework
{
    public static class Helper
    {
        /// <summary>
        /// 
        /// </summary>
        public static ImageAsset LoadImage(string path)
        {
            var name = Path.GetFileNameWithoutExtension(path);
            var source = new MagickImage(path);

            return new ImageAsset(name, source);
        }

        /// <summary>
        /// 
        /// </summary>
        public static Matrix4[] CreateCubeOrientations(Vector3 position)
        {
            return new Matrix4[]
            {
                Matrix4.LookAt(position, position + Vector3.UnitZ, Vector3.UnitY),
                Matrix4.LookAt(position, position + Vector3.UnitY, Vector3.UnitX),
                Matrix4.LookAt(position, position + Vector3.UnitX, Vector3.UnitY),
                Matrix4.LookAt(position, position + -Vector3.UnitZ, Vector3.UnitY),
                Matrix4.LookAt(position, position + -Vector3.UnitY, -Vector3.UnitX),
                Matrix4.LookAt(position, position + -Vector3.UnitX, Vector3.UnitY)
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public static Vector2[] VogelDisk(int count, float phi)
        {
            var result = new Vector2[count];

            for (int i = 0; i < count; i++)
                result[i] = VogelDisk(i, count, phi);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public static Vector2 VogelDisk(int index, int count, float phi)
        {
            var goldenAngle = 2.399963229f;
            var theta = index * goldenAngle + phi;
            var r = MathF.Sqrt(index + 0.5f) / MathF.Sqrt(count);
            return new Vector2(r * MathF.Cos(theta), r * MathF.Sin(theta));
        }

        /// <summary>
        /// 
        /// </summary>
        public static Vector3[] VogelCone(int count, float phi, float angle)
        {
            // vogel data
            var result = new Vector3[count];
            var goldenAngle = 2.399963229f;
            var zRange = 1f - MathF.Cos(angle);

            // vogel generation
            for (int i = 0; i < count; i++)
            {
                var theta = i * goldenAngle + phi;
                var z = 1f - (zRange * ((i + 0.5f) / count));
                var r = MathF.Sqrt(1f - (z * z));
                result[i] = new Vector3(r * MathF.Cos(theta), r * MathF.Sin(theta), z);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public static Vector3 VogelCone(int index, int count, float phi, Vector3 direction, float angle)
        {
            // rotation matrix
            var f = -direction;
            var s = Vector3.Cross(f, Vector3.UnitY);
            var u = Vector3.Cross(s, f);
            var rotationMatrix = new Matrix3(s, u, -f);

            var goldenAngle = 2.399963229f;
            var theta = index * goldenAngle + phi;

            var zRange = 1f - MathF.Cos(angle);
            var SqrtCount = MathF.Sqrt(count);
            var z = 1f - (zRange * MathF.Sqrt(index + 0.5f) / SqrtCount);
            var r = MathF.Sqrt(1f - (z * z));
            var result = new Vector3(r * MathF.Cos(theta), r * MathF.Sin(theta), z);

            return rotationMatrix * result;
        }
    }
}
