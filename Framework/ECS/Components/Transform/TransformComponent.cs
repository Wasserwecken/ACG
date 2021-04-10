using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Framework.ECS.Components.Transform
{
    [DebuggerDisplay("Position: {Position}, Forward: {Forward}, Scale: {Scale}")]
    public struct TransformComponent
    {
        /// <summary>
        /// Parent in world space
        /// </summary>
        public Matrix4 ParentSpace
        {
            get => _parentSpace;
            set
            {
                _parentSpace = value;
                _parentSpaceInverse = value.Inverted();
                _isWorldOutdated = true;
            }
        }

        public Matrix4 WorldSpace
        {
            get
            {
                UpdateWorldSpace();
                return _worldSpace;
            }
        }

        public Matrix4 WorldSpaceInverse
        {
            get
            {
                UpdateWorldSpace();
                return _worldSpaceInverse;
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
            get => (_localTranslationScaleSpace * ParentSpace).Row3.Xyz;
            set
            {
                var local = new Vector4(value, 1f) * _parentSpaceInverse;

                _localTranslationScaleSpace.M41 = local.X;
                _localTranslationScaleSpace.M42 = local.Y;
                _localTranslationScaleSpace.M43 = local.Z;

                _isLocalOutdated = true;
                _isWorldOutdated = true;
            }
        }

        public Vector3 Scale
        {
            get => (_localTranslationScaleSpace * ParentSpace).ClearRotation().Diagonal.Xyz;
            set
            {
                var local = new Vector4(value, 1f) * _parentSpaceInverse;

                _localTranslationScaleSpace.M11 = local.X;
                _localTranslationScaleSpace.M22 = local.Y;
                _localTranslationScaleSpace.M33 = local.Z;

                _isLocalOutdated = true;
                _isWorldOutdated = true;
            }
        }

        public Vector3 Forward
        {
            get => (_localRotationSpace * ParentSpace).ClearScale().Row2.Xyz;
            set
            {
                var local = (new Vector4(value, 1f) * _parentSpaceInverse).Xyz;

                var forward = local.Normalized();
                var right = Vector3.Cross(Vector3.UnitY, forward).Normalized();
                var up = Vector3.Cross(forward, right).Normalized();

                _localRotationSpace.Row0.Xyz = right;
                _localRotationSpace.Row1.Xyz = up;
                _localRotationSpace.Row2.Xyz = forward;

                _isLocalOutdated = true;
                _isWorldOutdated = true;
            }
        }

        public Vector3 Right
        {
            get => (_localRotationSpace * ParentSpace).ClearScale().Row0.Xyz;
        }

        public Vector3 Up
        {
            get => (_localRotationSpace * ParentSpace).ClearScale().Row1.Xyz;
        }

        private bool _isLocalOutdated;
        private bool _isWorldOutdated;
        private Matrix4 _localRotationSpace;
        private Matrix4 _localTranslationScaleSpace;
        private Matrix4 _localSpace;
        private Matrix4 _parentSpace;
        private Matrix4 _worldSpace;
        private Matrix4 _parentSpaceInverse;
        private Matrix4 _worldSpaceInverse;

        /// <summary>
        /// 
        /// </summary>
        public static TransformComponent Default => new TransformComponent()
        {
            ParentSpace = Matrix4.Identity,
            LocalSpace = Matrix4.Identity
        };

        /// <summary>
        /// 
        /// </summary>
        private void UpdateWorldSpace()
        {
            if (_isLocalOutdated || _isWorldOutdated)
            {
                _worldSpace = LocalSpace * ParentSpace;
                _worldSpaceInverse = _worldSpace.Inverted();
                _isWorldOutdated = false;
            }
        }
    }
}
