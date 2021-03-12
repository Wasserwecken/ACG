#version 430 core
in VertexOut
{
    vec2 UV0;
    vec2 UV1;
    vec4 Color;
    vec4 NormalLocal;
    vec4 NormalWorld;
    vec4 NormalView;
    vec4 PositionLocal;
    vec4 PositionView;
    vec4 PositionWorld;
} _vertex;

layout (std430) buffer ShaderTime {
 float Total;
 float Delta;
 float TotalSin;
 float TotalSin01;
} _time;

layout (std430) buffer ShaderRenderSpace {
    mat4 LocalToWorld;
    mat4 LocalToView;
    mat4 LocalToProjection;
    mat4 LocalToWorldRotation;
    mat4 LocalToViewRotation;
    mat4 WorldToView;
    vec3 ViewPosition;
    vec3 ViewDirection;
} _space;

uniform vec4 BaseColor;
uniform vec4 MREO;
uniform float Normal;

uniform sampler2D BaseColorMap;
uniform sampler2D MetallicRoughnessMap;
uniform sampler2D EmissiveMap;
uniform sampler2D OcclusionMap;
uniform sampler2D NormalMap;

out vec4 OutputColor;


void main()
{
    vec4 color = texture(BaseColorMap, _vertex.UV0) * BaseColor;


    vec3 surfaceNormal = abs(normalize(vec3(_vertex.NormalView)));
    vec3 surfaceColor = vec3(1.0);
    vec3 corrected = pow(surfaceNormal, vec3(0.454545454545));

    OutputColor = vec4(corrected, 1.0);
    OutputColor = color;
}