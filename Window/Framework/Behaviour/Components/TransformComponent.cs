using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public class TransformComponent
    {
        public Matrix4 Space => RotationSpace * ScaleSpace * TranslationSpace;
        public Matrix4 TranslationSpace { get; private set; }
        public Matrix4 RotationSpace { get; private set; }
        public Matrix4 ScaleSpace { get; private set; }
        private Matrix4 _rotationSpaceInverted;

        public Vector3 Position
        {
            get => TranslationSpace.Column3.Xyz;
            set => TranslationSpace = Matrix4.CreateTranslation(value);
        }

        public Vector3 Scale
        {
            get => ScaleSpace.Diagonal.Xyz;
            set => ScaleSpace = Matrix4.CreateScale(value);
        }

        public Vector3 Right
        {
            get => _rotationSpaceInverted.Row0.Xyz;
            set
            {
                value.Normalize();
                var forward = Vector3.Cross(Vector3.UnitY, value);
                var up = Vector3.Cross(forward, value);

                _rotationSpaceInverted.Row0.Xyz = value;
                _rotationSpaceInverted.Row1.Xyz = up;
                _rotationSpaceInverted.Row2.Xyz = forward;

                RotationSpace = _rotationSpaceInverted.Inverted();
            }
        }

        public Vector3 Up
        {
            get => _rotationSpaceInverted.Row1.Xyz;
            set
            {
                value.Normalize();
                var right = Vector3.Cross(Vector3.UnitX, value);
                var forward = Vector3.Cross(right, value);

                _rotationSpaceInverted.Row0.Xyz = right;
                _rotationSpaceInverted.Row1.Xyz = value;
                _rotationSpaceInverted.Row2.Xyz = forward;

                RotationSpace = _rotationSpaceInverted.Inverted();
            }
        }

        public Vector3 Forward
        {
            get => _rotationSpaceInverted.Row2.Xyz;
            set
            {
                var forward = value.Normalized();
                var right = Vector3.Cross(Vector3.UnitY, forward).Normalized();
                var up = Vector3.Cross(forward, right).Normalized();

                _rotationSpaceInverted.Row0.Xyz = right;
                _rotationSpaceInverted.Row1.Xyz = up;
                _rotationSpaceInverted.Row2.Xyz = forward;

                RotationSpace = _rotationSpaceInverted;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public TransformComponent()
        {
            Reset();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            TranslationSpace = Matrix4.CreateTranslation(0, 0, 0);
            RotationSpace = Matrix4.Identity;
            _rotationSpaceInverted = Matrix4.Identity;
            ScaleSpace = Matrix4.Identity;
        }
    }
}
