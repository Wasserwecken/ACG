#version 430 core
layout (location = 0) in vec3 BufferVertex;

layout (std430) buffer ShaderViewSpaceBlock {
    mat4 WorldToView;
    mat4 WorldToProjection;
    mat4 WorldToViewRotation;
    mat4 WorldToProjectionRotation;
    vec4 ViewPosition;
    vec4 ViewDirection;
    vec2 Resolution;
} _viewSpace;

out VertexUV
{
    vec3 UV0;
} _vertexUV;

void main()
{
    _vertexUV.UV0 = BufferVertex;
    gl_Position = _viewSpace.WorldToProjectionRotation * vec4(BufferVertex, 1.0);
    gl_Position = gl_Position.xyww;
}