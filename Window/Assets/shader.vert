#version 430 core

layout (location = 0) in vec3 BufferVertex;
layout (location = 1) in vec3 BufferNormal;
layout (location = 3) in vec2 BufferUV;

layout (std430) buffer ShaderTime {
 float Total;
 float Delta;
 float TotalSin;
 float TotalSin01;
} _time;

layout (std140) uniform ShaderSpace {
    mat4 LocalToWorld;
    mat4 LocalToView;
    mat4 LocalToProjection;
    mat4 LocalToWorldRotation;
    mat4 LocalToViewRotation;
    mat4 WorldToView;
    vec3 ViewPosition;
    vec3 ViewDirection;
} _space;

out VertexOut
{
    vec2 UV;
    vec4 NormalLocal;
    vec4 NormalWorld;
    vec4 NormalView;
    vec4 PositionLocal;
    vec4 PositionView;
    vec4 PositionWorld;
} _vertex;

void main(void)
{
    _vertex.UV = BufferUV;

    _vertex.NormalLocal = vec4(BufferNormal, 1.0);
    _vertex.NormalWorld = _space.LocalToWorldRotation * _vertex.NormalLocal;
    _vertex.NormalView = _space.LocalToViewRotation * _vertex.NormalLocal;

    _vertex.PositionLocal = vec4(BufferVertex, 1.0);
    _vertex.PositionWorld = _space.LocalToWorld * _vertex.PositionLocal;
    _vertex.PositionView = _space.LocalToView * _vertex.PositionLocal;
    gl_Position = _space.LocalToProjection * _vertex.PositionLocal;
}