﻿using OpenTK.Mathematics;

namespace Framework.Assets.Shader.Block.Data
{
    public struct ShaderRenderSpace
    {
        public Matrix4 LocalToWorld;
        public Matrix4 LocalToView;
        public Matrix4 LocalToProjection;
        public Matrix4 LocalToWorldRotation;
        public Matrix4 LocalToViewRotation;
        public Matrix4 WorldToView;
        public Matrix4 ProjectionSpace;
        public Vector4 ViewPosition;
        public Vector4 ViewDirection;
    }
}
