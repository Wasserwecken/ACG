#version 330 core

layout (location = 0) in vec3 vertex;
layout (location = 1) in vec3 normal;
layout (location = 2) in vec4 color;
layout (location = 3) in vec2 uv;

uniform float time;

out vec2 texCoord;

void main(void)
{
    texCoord = uv;
    gl_Position = vec4(vertex, 1.0);
}