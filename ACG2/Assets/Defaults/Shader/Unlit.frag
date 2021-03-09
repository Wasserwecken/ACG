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

struct MaterialSettings
{
    vec3 Albedo;
    vec3 Emissive;
    float Metallic;
    float Roughness;
    float Normal;
    float Occlusion;
};

uniform sampler2D AlbedoMap;
uniform sampler2D MetallicMap;
uniform sampler2D RoughnessMap;
uniform sampler2D NormalMap;
uniform sampler2D OcclusionMap;
uniform sampler2D EmissiveMap;

out vec4 OutputColor;


void main()
{
    vec3 surfaceNormal = normalize(vec3(_vertex.NormalView));
    vec3 surfaceColor = vec3(1.0);
    vec3 corrected = pow(surfaceColor, vec3(0.454545454545));

    OutputColor = vec4(corrected, 1.0);
}