#version 330 core

layout (location = 0) in vec3 BufferVertex;
layout (location = 1) in vec3 BufferNormal;
layout (location = 2) in vec2 BufferUV;


uniform float TimeTotal;
uniform float TimeDelta;

uniform mat4 LocalToWord;
uniform mat4 LocalToProjection;


out vec3 VertexNormal;
out vec2 VertexUV;
out vec4 LocalPosition;
out vec4 WorldPosition;


void main(void)
{
    VertexNormal = BufferNormal;
    VertexUV = BufferUV;

    LocalPosition = vec4(BufferVertex, 1.0);
    WorldPosition = LocalToWord * LocalPosition;
    gl_Position = LocalToProjection * LocalPosition;
}