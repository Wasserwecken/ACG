#version 430 core
layout (location = 0) in vec3 BufferVertex;

layout (std430) buffer ShaderViewSpace {
    mat4 WorldToView;
    mat4 WorldToViewRotation;
    mat4 WorldToProjection;
    mat4 WorldToProjectionRotation;
    vec4 ViewPosition;
    vec4 ViewDirection;
} _viewSpace;

out VertexOut
{
    vec3 UV0;
} _vertex;

void main()
{
    _vertex.UV0 = BufferVertex;
    gl_Position = _viewSpace.WorldToProjectionRotation * vec4(BufferVertex, 1.0);
    gl_Position = gl_Position.xyww;
}