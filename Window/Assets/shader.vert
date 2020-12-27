#version 330 core

layout (location = 0) in vec3 BufferVertex;
layout (location = 1) in vec3 BufferNormal;
layout (location = 2) in vec2 BufferUV;

uniform vec3 ViewPosition;
uniform float TimeTotal;
uniform float TimeDelta;

uniform mat4 LocalToWorldSpace;
uniform mat4 LocalToViewSpace;
uniform mat4 LocalToProjectionSpace;
uniform mat3 LocalToWorldRotationSpace;
uniform mat3 LocalToViewRotationSpace;

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
    vertexOut.NormalWorld = LocalToWorldRotationSpace * BufferNormal;
    vertexOut.NormalView = LocalToViewRotationSpace * BufferNormal;

    vertexOut.PositionLocal = vec4(BufferVertex, 1.0);
    vertexOut.PositionWorld = LocalToProjectionSpace * vertexOut.PositionLocal;
    vertexOut.PositionView = LocalToViewSpace * vertexOut.PositionLocal;
    gl_Position = LocalToProjectionSpace * vertexOut.PositionLocal;
}