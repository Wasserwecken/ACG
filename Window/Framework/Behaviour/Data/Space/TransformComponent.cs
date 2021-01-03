using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public struct TransformComponent : IEntityComponent
    {
        public static TransformComponent Default => new TransformComponent()
        {
            TranslationSpace = Matrix4.CreateTranslation(0, 0, 0),
            RotationSpace = Matrix4.Identity,
            ScaleSpace = Matrix4.Identity
        };


        public Matrix4 Space => RotationSpace * ScaleSpace * TranslationSpace;

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

        public Vector3 Forward
        {
            get => RotationSpace.Row2.Xyz;
            set
            {
                var forward = value.Normalized();
                var right = Vector3.Cross(Vector3.UnitY, forward).Normalized();
                var up = Vector3.Cross(forward, right).Normalized();

                RotationSpace = CreateRotationMatrix(right, up, forward);
            }
        }

        public Matrix4 TranslationSpace { get; set; }
        public Matrix4 RotationSpace { get; set; }
        public Matrix4 ScaleSpace { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TransformComponent(Vector3 position)
        {
            this = Default;
            Position = position;
        }

        /// <summary>
        /// 
        /// </summary>
        private Matrix4 CreateRotationMatrix(Vector3 right, Vector3 up, Vector3 forward)
        {
            var matrix = Matrix4.Identity;
            
            matrix.Row0.Xyz = right;
            matrix.Row1.Xyz = up;
            matrix.Row2.Xyz = forward;

            return matrix;
        }
    }
}
