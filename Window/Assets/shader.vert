#version 330 core

in vec3 vertices;
in vec2 uv;

out vec2 texCoord;

void main(void)
{
    texCoord = uv;
    gl_Position = vec4(vertices, 1.0);
}