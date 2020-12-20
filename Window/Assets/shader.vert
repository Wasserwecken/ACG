#version 330 core

layout (location = 0) in vec3 BufferVertex;
layout (location = 1) in vec3 BufferNormal;
layout (location = 2) in vec4 BufferColor;
layout (location = 3) in vec2 BufferUV;


uniform float TimeTotal;
uniform float TimeDelta;

uniform mat4 WorldSpace;
uniform mat4 ViewSpace;
uniform mat4 ProjectionSpace;


out vec3 VertexNormal;
out vec4 VertexColor;
out vec2 VertexUV;

out vec4 LocalPosition;
out vec4 ViewPosition;
out vec4 WorldPosition;


void main(void)
{
    VertexNormal = BufferNormal;
    VertexColor = BufferColor;
    VertexUV = BufferUV;

    vec4 vertex = vec4(BufferVertex, 1.0);
    WorldPosition = WorldSpace * vertex;
    ViewPosition =  ViewSpace * vertex;
    gl_Position = ProjectionSpace * vertex;
}