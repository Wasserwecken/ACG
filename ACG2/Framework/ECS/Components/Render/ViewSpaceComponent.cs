using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Framework.ECS.Components.Render
{
    [DebuggerDisplay("WorldToView: {WorldToView}, WorldToProjection: {WorldToProjection}")]
    public class ViewSpaceComponent : IComponent
    {
        public Matrix4 WorldToView;
        public Matrix4 WorldToProjection;
        public Matrix4 WorldToViewRotation;
        public Vector3 ViewPosition;
        public Vector3 ViewDirection;
    }
}
