#version 430 core

layout (location = 0) in vec3 BufferVertex;
layout (location = 1) in vec3 BufferNormal;
layout (location = 2) in vec3 BufferTangent;
layout (location = 3) in vec2 BufferUV0;
layout (location = 4) in vec2 BufferUV1;
layout (location = 5) in vec4 BufferColor;

layout (std430) buffer ShaderTime {
    float Frame;
    float Fixed;
    float Total;
    float TotalSin;
} _time;

layout (std430) buffer ShaderPrimitiveSpace {
    mat4 LocalToWorld;
    mat4 LocalToView;
    mat4 LocalToProjection;
    mat4 LocalToWorldRotation;
    mat4 LocalToViewRotation;
    mat4 LocalToProjectionRotation;
} _primitiveSpace;

layout (std430) buffer ShaderViewSpace {
    mat4 WorldToView;
    mat4 WorldToViewRotation;
    mat4 WorldToProjection;
    mat4 WorldToProjectionRotation;
    vec3 ViewPosition;
    vec3 ViewDirection;
} _viewSpace;

out VertexOut
{
    vec2 UV0;
    vec2 UV1;
    vec4 Color;
    vec4 NormalLocal;
    vec4 NormalWorld;
    vec4 NormalView;
    vec4 PositionLocal;
    vec4 PositionView;
    vec4 PositionWorld;
} _vertex;

void main(void)
{
    _vertex.UV0 = BufferUV0;
    _vertex.UV1 = BufferUV1;

    _vertex.Color = BufferColor;

    _vertex.NormalLocal = vec4(BufferNormal, 1.0);
    _vertex.NormalWorld = _primitiveSpace.LocalToWorldRotation * _vertex.NormalLocal;
    _vertex.NormalView = _primitiveSpace.LocalToViewRotation * _vertex.NormalLocal;

    _vertex.PositionLocal = vec4(BufferVertex, 1.0);
    _vertex.PositionWorld = _primitiveSpace.LocalToWorld * _vertex.PositionLocal;
    _vertex.PositionView = _primitiveSpace.LocalToView * _vertex.PositionLocal;
    gl_Position = _primitiveSpace.LocalToProjection * _vertex.PositionLocal;
}