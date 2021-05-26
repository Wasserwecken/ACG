#version 430 core
// INPUT VERTICIES
layout (location = 0) in vec3 BufferVertex;
layout (location = 1) in vec3 BufferNormal;
layout (location = 2) in vec4 BufferTangent;
layout (location = 3) in vec2 BufferUV0;
layout (location = 4) in vec2 BufferUV1;
layout (location = 5) in vec4 BufferColor;

// INPUT GLOBAL UNIFORMS
layout (std430) buffer ShaderTimeBlock {
    float Total;
    float TotalSin;
    float Frame;
    float Fixed;
} _time;

layout (std430) buffer ShaderPrimitiveSpaceBlock {
    mat4 LocalToWorld;
    mat4 LocalToWorldRotation;
} _primitiveSpace;

layout (std430) buffer ShaderViewSpaceBlock {
    mat4 Projection;
    mat4 WorldToView;
    mat4 WorldToProjection;
    mat4 WorldToViewRotation;
    mat4 WorldToProjectionRotation;
    vec4 ViewPosition;
    vec4 ViewDirection;
    vec2 Resolution;
} _viewSpace;

// SHADER OUTPUT
out VertexSpace
{
    mat4 LocalToView;
    mat4 LocalToProjection;
    mat4 LocalToViewRotation;
    mat4 LocalToProjectionRotation;
} _vertexSpace;

out VertexPosition
{
    vec4 PositionLocal;
    vec4 PositionView;
    vec4 PositionWorld;
} _vertexPosition;

out VertexNormal
{
    vec3 NormalLocal;
    vec3 NormalWorld;
    vec3 NormalView;
} _vertexNormal;

out VertexTangent
{
    mat3 TangentSpaceLocal;
    mat3 TangentSpaceWorld;
    mat3 TangentSpaceView;
} _vertexTangent;

out VertexUV
{
    vec2 UV0;
    vec2 UV1;
} _vertexUV;

out VertexColor
{
    vec4 Color;
} _vertexColor;

// LOGIC
void main(void)
{
    _vertexUV.UV0 = BufferUV0;
    _vertexUV.UV1 = BufferUV1;

    _vertexColor.Color = BufferColor;
    
    _vertexSpace.LocalToView = _viewSpace.WorldToView * _primitiveSpace.LocalToWorld;
    _vertexSpace.LocalToProjection = _viewSpace.WorldToProjection * _primitiveSpace.LocalToWorld;
    _vertexSpace.LocalToViewRotation = _viewSpace.WorldToViewRotation * _primitiveSpace.LocalToWorld;
    _vertexSpace.LocalToProjectionRotation = _viewSpace.WorldToProjectionRotation * _primitiveSpace.LocalToWorld;

    _vertexNormal.NormalLocal = BufferNormal;
    _vertexNormal.NormalWorld = mat3(_primitiveSpace.LocalToWorldRotation) * _vertexNormal.NormalLocal;
    _vertexNormal.NormalView = mat3(_vertexSpace.LocalToViewRotation) * _vertexNormal.NormalLocal;

    vec3 tangent = (BufferTangent * BufferTangent.w).xyz;
    vec3 bitangent = cross(BufferNormal, tangent);
    _vertexTangent.TangentSpaceLocal = mat3(tangent, bitangent, _vertexNormal.NormalLocal);
    _vertexTangent.TangentSpaceWorld = mat3(_primitiveSpace.LocalToWorldRotation) * _vertexTangent.TangentSpaceLocal;
    _vertexTangent.TangentSpaceView = mat3(_vertexSpace.LocalToViewRotation) * _vertexTangent.TangentSpaceLocal;

    _vertexPosition.PositionLocal = vec4(BufferVertex, 1.0);
    _vertexPosition.PositionWorld = _primitiveSpace.LocalToWorld * _vertexPosition.PositionLocal;
    _vertexPosition.PositionView = _vertexSpace.LocalToView * _vertexPosition.PositionLocal;

    gl_Position = _vertexSpace.LocalToProjection * _vertexPosition.PositionLocal;
}