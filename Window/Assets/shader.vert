#version 430 core

layout (location = 0) in vec3 BufferVertex;
layout (location = 1) in vec3 BufferNormal;
layout (location = 2) in vec2 BufferUV;

layout (std430) buffer TimeBlock {
 float Total;
 float Delta;
} _time;

uniform SpaceBlock {
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
} vertexOut;

void main(void)
{
    vertexOut.UV = BufferUV;

    vertexOut.NormalLocal = BufferNormal;
    vertexOut.NormalWorld = _space.LocalToWorldRotation * BufferNormal;
    vertexOut.NormalView = _space.LocalToViewRotation * BufferNormal;

    vertexOut.PositionLocal = vec4(BufferVertex, 1.0);
    vertexOut.PositionWorld = _space.LocalToWorld * vertexOut.PositionLocal;
    vertexOut.PositionView = _space.LocalToView * vertexOut.PositionLocal;
    gl_Position = _space.LocalToProjection * vertexOut.PositionLocal;
}