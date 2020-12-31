#version 430 core

layout (location = 0) in vec3 BufferVertex;
layout (location = 1) in vec3 BufferNormal;
layout (location = 2) in vec2 BufferUV;

layout (std430) buffer TimeData {
 float Total;
 float Delta;
} _time;

uniform RenderSpaceData {
    mat4 LocalToWorld;
    mat4 LocalToView;
    mat4 LocalToProjection;
    mat3 LocalToWorldRotation;
    mat3 LocalToViewRotation;
} _space;

out VertexOut
{
    vec2 UV;
    vec3 NormalLocal;
    vec3 NormalWorld;
    vec3 NormalView;
    vec4 PositionLocal;
    vec4 PositionView;
    vec4 PositionWorld;
} _vertex;

void main(void)
{
    _vertex.UV = BufferUV;

    _vertex.NormalLocal = BufferNormal;
    _vertex.NormalWorld = _space.LocalToWorldRotation * BufferNormal;
    _vertex.NormalView = _space.LocalToViewRotation * BufferNormal;

    _vertex.PositionLocal = vec4(BufferVertex, 1.0);
    _vertex.PositionWorld = _space.LocalToWorld * _vertex.PositionLocal;
    _vertex.PositionView = _space.LocalToView * _vertex.PositionLocal;
    gl_Position = _space.LocalToProjection * _vertex.PositionLocal;
}