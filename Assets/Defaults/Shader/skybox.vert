#version 430 core
layout (location = 0) in vec3 BufferVertex;

layout (std430) buffer ShaderViewSpace {
    mat4 WorldToView;
    mat4 WorldToViewInverse;
    mat4 WorldToProjection;
    mat4 WorldToViewRotation;
    mat4 WorldToProjectionRotation;
    vec3 ViewPosition;
    vec3 ViewDirection;
    mat4 ViewProjection;
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