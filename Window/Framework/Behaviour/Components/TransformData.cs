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
            _translationSpace = Matrix4.CreateTranslation(0, 0, 0),
            _rotationSpace = Matrix4.Identity,
            _scaleSpace = Matrix4.Identity
        };


        public Matrix4 Space => _rotationSpace * _scaleSpace * _translationSpace;

        public Vector3 Position
        {
            get => _translationSpace.Row3.Xyz;
            set => _translationSpace = Matrix4.CreateTranslation(value);
        }

        public Vector3 Scale
        {
            get => _scaleSpace.Diagonal.Xyz;
            set => _scaleSpace = Matrix4.CreateScale(value);
        }

        public Vector3 Right
        {
            get => _rotationSpace.Row0.Xyz;
            set
            {
                var right = value.Normalized();
                var forward = Vector3.Cross(right, Vector3.UnitY).Normalized();
                var up = Vector3.Cross(forward, right).Normalized();

                _rotationSpace = CreateRotationMatrix(right, up, forward);
            }
        }

        public Vector3 Up
        {
            get => _rotationSpace.Row1.Xyz;
            set
            {
                var up = value.Normalized();
                var forward = Vector3.Cross(Vector3.UnitX, up).Normalized();
                var right = Vector3.Cross(up, forward).Normalized();
                _rotationSpace = CreateRotationMatrix(right, up, forward);
            }
        }

        public Vector3 Forward
        {
            get => _rotationSpace.Row2.Xyz;
            set
            {
                var forward = value.Normalized();
                var right = Vector3.Cross(Vector3.UnitY, forward).Normalized();
                var up = Vector3.Cross(forward, right).Normalized();

                _rotationSpace = CreateRotationMatrix(right, up, forward);
            }
        }

        private Matrix4 _translationSpace;
        private Matrix4 _rotationSpace;
        private Matrix4 _scaleSpace;

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
