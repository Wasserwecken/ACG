#version 430 core
// CONSTANTS
const float SQRT205 = 0.70710678118;
const float SQRT2 = 1.41421356237;
const float SQRT305 = 0.86602540378;
const float SQRT3 = 1.73205080756;
const float PI025 = 0.78539816339;
const float PI05 = 1.57079632674;
const float PI = 3.14159265359;
const float PI2 = 6.28318530718;
const float RADTODEG = 57.295779513;
const float DEGTORAD = 0.01745329252;

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

// INPUT GLOBAL UNIFORMS
layout (std430) buffer ShaderTimeBlock {
    float Total;
    float TotalSin;
    float Frame;
    float Fixed;
} _time;

layout (std430) buffer ShaderPrimitiveSpace {
    mat4 LocalToWorld;
    mat4 LocalToWorldRotation;
} _primitiveSpace;

layout (std430) buffer ShaderViewSpaceBlock {
    mat4 WorldToView;
    mat4 WorldToProjection;
    mat4 WorldToViewRotation;
    mat4 WorldToProjectionRotation;
    vec4 ViewPosition;
    vec4 ViewDirection;
    vec2 Resolution;
} _viewSpace;

// INPUT GENERAL UNIFORMS
uniform float AlphaCutoff;

// INPUT SPECIFIC UNIFORMS
uniform vec4 BaseColor;
uniform vec4 MREO;
uniform float Normal;
uniform sampler2D BaseColorMap;
uniform sampler2D MetallicRoughnessMap;
uniform sampler2D EmissiveMap;
uniform sampler2D OcclusionMap;
uniform sampler2D NormalMap;

// SHADER OUTPUT
out vec4 DeferredPosition;
out vec3 DeferredAlbedo;
out vec3 DeferredNormalSurface;
out vec3 DeferredNormalTexture;
out vec3 DeferredMRO;
out vec3 DeferredEmission;

// MAIN
void main()
{
    vec4 baseColor = pow(texture(BaseColorMap, _vertexUV.UV0), vec4(2.2));

   if (baseColor.w < AlphaCutoff)
        discard;
    
    vec2 metallicRoughness = texture(MetallicRoughnessMap, _vertexUV.UV0).zy * MREO.xy;

    DeferredPosition = _vertexPosition.PositionWorld;
    DeferredAlbedo = baseColor.xyz;
    DeferredNormalSurface = _vertexNormal.NormalWorld;
    DeferredNormalTexture = (texture(NormalMap, _vertexUV.UV0).xyz * 2.0 - 1.0) * vec3(1.0, 1.0, 0.5 / Normal);
    DeferredMRO = vec3(metallicRoughness, texture(OcclusionMap, _vertexUV.UV0).x * MREO.w);
    DeferredEmission = texture(EmissiveMap, _vertexUV.UV0).xyz * MREO.z;
}