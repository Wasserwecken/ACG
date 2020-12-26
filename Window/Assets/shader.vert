#version 330 core

layout (location = 0) in vec3 BufferVertex;
layout (location = 1) in vec3 BufferNormal;
layout (location = 2) in vec2 BufferUV;


uniform float TimeTotal;
uniform float TimeDelta;

uniform mat4 LocalToWord;
uniform mat4 LocalToProjection;


out vec3 VertexNormalLocal;
out vec3 VertexNormalProjection;
out vec2 VertexUV;
out vec4 PositionLocal;
out vec4 PositionWorld;


void main(void)
{
    VertexNormalLocal = BufferNormal;
    VertexNormalProjection = mat3(LocalToProjection) * BufferNormal;
    VertexUV = BufferUV;

    PositionLocal = vec4(BufferVertex, 1.0);
    PositionWorld = LocalToWord * PositionLocal;
    gl_Position = LocalToProjection * PositionLocal;
}