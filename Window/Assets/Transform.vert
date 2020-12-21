#version 330 core

layout (location = 0) in vec3 BufferVertex;
layout (location = 1) in vec4 BufferColor;

uniform mat4 ProjectionSpace;

out vec4 VertexColor;

void main(void)
{
    VertexColor = BufferColor;
    gl_Position = ProjectionSpace * vec4(BufferVertex, 1.0);
}