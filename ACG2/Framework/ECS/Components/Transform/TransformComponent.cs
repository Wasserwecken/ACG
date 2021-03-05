using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Framework.ECS.Components.Transform
{
    [DebuggerDisplay("Position: {Position}, Forward: {Forward}, Scale: {Scale}")]
    public class TransformComponent : IComponent
    {
        public Matrix4 ParentSpace
        {
            get => _parentSpace;
            set
            {
                _parentSpace = value;
                _isWorldOutdated = true;
            }
        }

        public Matrix4 WorldSpace
        {
            get
            {
                if (_isLocalOutdated || _isWorldOutdated)
                {
                    _worldSpace = ParentSpace * LocalSpace;
                    _isWorldOutdated = false;
                }

                return _worldSpace;
            }
            set
            {
                _worldSpace = value;
                _localSpace = _worldSpace.Inverted() * _parentSpace;
                _isWorldOutdated = false;
                _isLocalOutdated = false;
            }
        }

        public Matrix4 LocalSpace
        {
            get
            {
                if (_isLocalOutdated)
                {
                    _localSpace = _localRotationSpace * _localTranslationScaleSpace;
                    _isLocalOutdated = false;
                }

                return _localSpace;
            }
            set
            {
                _localSpace = value;
                _localTranslationScaleSpace = _localSpace.ClearRotation();
                _localRotationSpace = _localSpace.ClearScale().ClearTranslation();

                _isLocalOutdated = false;
                _isWorldOutdated = true;
            }
        }

        public Vector3 Position
        {
            get => _localTranslationScaleSpace.Row3.Xyz;
            set
            {
                _localTranslationScaleSpace.Row3.X = value.X;
                _localTranslationScaleSpace.Row3.Y = value.Y;
                _localTranslationScaleSpace.Row3.Z = value.Z;
                _localTranslationScaleSpace.Row3.W = 1f;

                _isLocalOutdated = true;
                _isWorldOutdated = true;
            }
        }

        public Vector3 Scale
        {
            get => _localTranslationScaleSpace.Diagonal.Xyz;
            set
            {
                _localTranslationScaleSpace.M11 = value.X;
                _localTranslationScaleSpace.M22 = value.Y;
                _localTranslationScaleSpace.M33 = value.Z;
                _localTranslationScaleSpace.M44 = 0f;

                _isLocalOutdated = true;
                _isWorldOutdated = true;
            }
        }

        public Vector3 Forward
        {
            get => _localRotationSpace.Row2.Xyz;
            set
            {
                var forward = value.Normalized();
                var right = Vector3.Cross(Vector3.UnitY, forward).Normalized();
                var up = Vector3.Cross(forward, right).Normalized();

                _localRotationSpace.Row0.Xyz = right;
                _localRotationSpace.Row1.Xyz = up;
                _localRotationSpace.Row2.Xyz = forward;

                _isLocalOutdated = true;
                _isWorldOutdated = true;
            }
        }

        private bool _isLocalOutdated;
        private bool _isWorldOutdated;
        private Matrix4 _localRotationSpace;
        private Matrix4 _localTranslationScaleSpace;
        private Matrix4 _localSpace;
        private Matrix4 _parentSpace;
        private Matrix4 _worldSpace;

        /// <summary>
        /// 
        /// </summary>
        public TransformComponent()
        {
            ParentSpace = Matrix4.Identity;
            LocalSpace = Matrix4.Identity;
        }
    }
}
