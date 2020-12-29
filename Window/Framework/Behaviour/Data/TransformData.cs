using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public struct TransformData
    {
        public static TransformData Default => new TransformData()
        {
            TranslationSpace = Matrix4.CreateTranslation(0, 0, 0),
            RotationSpace = Matrix3.Identity,
            ScaleSpace = Matrix4.Identity
        };


        public Matrix4 Space => new Matrix4(RotationSpace) * ScaleSpace * TranslationSpace;

        public Vector3 Position
        {
            get => TranslationSpace.Row3.Xyz;
            set => TranslationSpace = Matrix4.CreateTranslation(value);
        }

        public Vector3 Scale
        {
            get => ScaleSpace.Diagonal.Xyz;
            set => ScaleSpace = Matrix4.CreateScale(value);
        }

        public Vector3 Right
        {
            get => RotationSpace.Row0;
            set
            {
                var right = value.Normalized();
                var forward = Vector3.Cross(right, Vector3.UnitY).Normalized();
                var up = Vector3.Cross(forward, right).Normalized();

                RotationSpace = CreateRotationMatrix(right, up, forward);
            }
        }

        public Vector3 Up
        {
            get => RotationSpace.Row1;
            set
            {
                var up = value.Normalized();
                var forward = Vector3.Cross(Vector3.UnitX, up).Normalized();
                var right = Vector3.Cross(up, forward).Normalized();
                RotationSpace = CreateRotationMatrix(right, up, forward);
            }
        }

        public Vector3 Forward
        {
            get => RotationSpace.Row2;
            set
            {
                var forward = value.Normalized();
                var right = Vector3.Cross(Vector3.UnitY, forward).Normalized();
                var up = Vector3.Cross(forward, right).Normalized();

                RotationSpace = CreateRotationMatrix(right, up, forward);
            }
        }

        public Matrix4 TranslationSpace { get; set; }
        public Matrix3 RotationSpace { get; set; }
        public Matrix4 ScaleSpace { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TransformData(Vector3 position)
        {
            this = Default;
            Position = position;
        }

        /// <summary>
        /// 
        /// </summary>
        private Matrix3 CreateRotationMatrix(Vector3 right, Vector3 up, Vector3 forward)
        {
            var matrix = Matrix3.Identity;
            
            matrix.Row0 = right;
            matrix.Row1 = up;
            matrix.Row2 = forward;

            return matrix;
        }
    }
}
