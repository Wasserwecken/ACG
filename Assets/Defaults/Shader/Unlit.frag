#version 430 core
// INPUT VERTEX
in VertexPosition
{
    vec4 PositionLocal;
    vec4 PositionView;
    vec4 PositionWorld;
} _vertexPosition;

in VertexNormal
{
    vec3 NormalLocal;
    vec3 NormalWorld;
    vec3 NormalView;
} _vertexNormal;

in VertexTangent
{
    mat3 TangentSpaceLocal;
    mat3 TangentSpaceWorld;
    mat3 TangentSpaceView;
} _vertexTangent;

in VertexUV
{
    vec2 UV0;
    vec2 UV1;
} _vertexUV;

in VertexColor
{
    vec4 Color;
} _vertexColor;

// SHADER BLOCK GLOBAL INPUT
layout (std430) buffer ShaderTime {
    float Frame;
    float Fixed;
    float Total;
    float TotalSin;
} _time;

layout (std430) buffer ShaderPrimitiveSpace {
    mat4 LocalToWorld;
    mat4 LocalToView;
    mat4 LocalToProjection;
    mat4 LocalToWorldRotation;
    mat4 LocalToViewRotation;
    mat4 LocalToProjectionRotation;
} _primitiveSpace;

layout (std430) buffer ShaderViewSpace {
    mat4 WorldToView;
    mat4 WorldToProjection;
    mat4 WorldToViewRotation;
    mat4 WorldToProjectionRotation;
    vec3 ViewPosition;
    vec3 ViewDirection;
} _viewSpace;

// MATERIAL INPUT
uniform vec4 BaseColor;
uniform vec4 MREO;
uniform float Normal;
uniform sampler2D BaseColorMap;
uniform sampler2D MetallicRoughnessMap;
uniform sampler2D EmissiveMap;
uniform sampler2D OcclusionMap;
uniform sampler2D NormalMap;

// FRAG OUT
out vec4 OutputColor;


void main()
{
    vec4 color = texture(BaseColorMap, _vertexUV.UV0) * BaseColor;

    vec3 surfaceNormal = abs(normalize(vec3(_vertexNormal.NormalView)));
    vec3 surfaceColor = vec3(1.0);
    vec3 corrected = pow(surfaceNormal, vec3(0.454545454545));

    OutputColor = vec4(corrected, 1.0);
    OutputColor = color;
}