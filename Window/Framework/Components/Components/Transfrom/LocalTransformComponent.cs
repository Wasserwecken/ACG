using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Framework
{
    [DebuggerDisplay("Position: {Position}, Forward: {Forward}, Scale: {Scale}")]
    public struct LocalTransformComponent : IEntityComponent
    {
        public static LocalTransformComponent Default => new LocalTransformComponent()
        {
            _translationScaleSpace = Matrix4.Identity,
            _rotationSpace = Matrix4.Identity,
            _totalSpace = Matrix4.Identity
        };

        public Matrix4 Space
        {
            get => _totalSpace;
            set
            {
                _totalSpace = value;

                _translationScaleSpace = _totalSpace.ClearRotation();
                _rotationSpace = _totalSpace.ClearScale().ClearTranslation();
            }
        }

        public Matrix4 RotationSpace
        {
            get => _rotationSpace;
            set
            {
                _rotationSpace = value;
                _totalSpace = _rotationSpace * _translationScaleSpace;
            }
        }

        public Vector3 Position
        {
            get => _translationScaleSpace.Row3.Xyz;
            set
            {
                _translationScaleSpace.Row3.X = value.X;
                _translationScaleSpace.Row3.Y = value.Y;
                _translationScaleSpace.Row3.Z = value.Z;
                _translationScaleSpace.Row3.W = 1f;

                _totalSpace = _rotationSpace * _translationScaleSpace;
            }
        }
        public Vector3 Scale
        {
            get => _translationScaleSpace.Diagonal.Xyz;
            set
            {
                _translationScaleSpace.M11 = value.X;
                _translationScaleSpace.M22 = value.Y;
                _translationScaleSpace.M33 = value.Z;
                _translationScaleSpace.M44 = 0f;

                _totalSpace = _rotationSpace * _translationScaleSpace;
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

                _rotationSpace.Row0.Xyz = right;
                _rotationSpace.Row1.Xyz = up;
                _rotationSpace.Row2.Xyz = forward;

                _totalSpace = _rotationSpace * _translationScaleSpace;
            }
        }

        private Matrix4 _rotationSpace;
        private Matrix4 _translationScaleSpace;
        private Matrix4 _totalSpace;

        /// <summary>
        /// 
        /// </summary>
        public LocalTransformComponent(Vector3 position)
        {
            this = Default;
            Position = position;
        }
    }
}
