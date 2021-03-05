#version 430 core
layout (location = 0) in vec3 BufferVertex;

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
    vec3 UV0;
} _vertex;

void main()
{
    _vertex.UV0 = BufferVertex;
    gl_Position = _space.LocalToProjection * vec4(BufferVertex, 1.0);
}  