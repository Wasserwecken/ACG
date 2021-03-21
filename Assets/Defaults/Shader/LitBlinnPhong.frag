#version 430 core
// VERTEX INPUT
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

// GLOBAL INPUT
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
    mat4 WorldToViewRotation;
    mat4 WorldToProjection;
    mat4 WorldToProjectionRotation;
    vec3 ViewPosition;
    vec3 ViewDirection;
} _viewSpace;

// POINT LIGHT INPUT
struct DirectionalLight
{
 vec4 Color;
 vec3 Direction;
};

struct PointLight
{
 vec4 Color;
 vec3 Position;
};

struct SpotLight
{
 vec4 Color;
 vec4 Position;
 vec4 Direction;
};

layout (std430) buffer ShaderDirectionalLight {
    DirectionalLight _directionalLights[];
};

layout (std430) buffer ShaderPointLight {
    PointLight _pointLights[];
};

layout (std430) buffer ShaderSpotLight {
    SpotLight _spotLights[];
};

// ENVIRONMENT
uniform samplerCube ReflectionMap;

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


vec3 blinn_phong(vec3 surfaceDiffuse, vec3 surfaceSpecular, float glossy, vec3 normal, vec3 halfway, vec3 lightDirection, vec3 lightColor)
{
    float luminance = max(dot(normal, lightDirection), 0.0);
    vec3 specular = surfaceSpecular * pow(max(dot(normal, halfway), 0.0), glossy);

    return (surfaceDiffuse + specular) * lightColor * luminance;
}

vec3 evaluate_lights(vec3 baseColor, float metalic, float roughness, vec3 surfaceNormal)
{
    vec3 viewDirection = _viewSpace.ViewPosition - _vertex.PositionWorld.xyz;
    vec3 reflectionColor = texture(ReflectionMap, reflect(-viewDirection, surfaceNormal)).xyz;
    vec3 specularColor = mix(vec3(1.0), baseColor, metalic);
    float glossy = (1.0 - (roughness.x * roughness.x)) * 128.0;
    vec3 result = reflectionColor * baseColor * (1.0 - roughness) * metalic;
    
    for(int i = 0; i < _directionalLights.length(); i++)
    {
        vec3 lightColor = _directionalLights[i].Color.xyz;
        vec3 lightDirection = normalize(-_directionalLights[i].Direction);
        vec3 halfwayDirection = normalize(lightDirection + viewDirection);

        result += blinn_phong(baseColor, specularColor, glossy, surfaceNormal, halfwayDirection, lightDirection, lightColor);
        result += _directionalLights[i].Color.w * baseColor * lightColor;
    }
    
    for(int i = 0; i < _pointLights.length(); i++)
    {
        vec3 lightDiff = _pointLights[i].Position - _vertex.PositionWorld.xyz;
        vec3 lightColor = _pointLights[i].Color.xyz;
        float lightDistance = length(lightDiff);
        vec3 lightDirection = normalize(lightDiff);
        vec3 halfwayDirection = normalize(lightDirection + _viewSpace.ViewDirection);
        float attenuationSqrared = 1.0 / (1.0 + (lightDistance * lightDistance));
        float attenuationLinear = 1.0 / (1.0 + lightDistance);

        result += blinn_phong(baseColor, specularColor, glossy, surfaceNormal, halfwayDirection, lightDirection, lightColor) * attenuationSqrared;
        result += _pointLights[i].Color.w * baseColor * lightColor * attenuationLinear;
    }   
    
    for(int i = 0; i < _spotLights.length(); i++)
    {
        vec3 lightDiff = _spotLights[i].Position.xyz - _vertex.PositionWorld.xyz;
        vec3 lightColor = _spotLights[i].Color.xyz;
        float lightDistance = length(lightDiff);
        vec3 lightDirection = normalize(lightDiff);
        vec3 halfwayDirection = normalize(lightDirection + _viewSpace.ViewDirection);
        float attenuationSqrared = 1.0 / (1.0 + (lightDistance * lightDistance));
        float attenuationLinear = 1.0 / (1.0 + lightDistance);

        float outerAngle = _spotLights[i].Position.w;
        float innerAngle = _spotLights[i].Direction.w;
        float theta = dot(lightDirection, normalize(_spotLights[i].Direction.xyz));
        float epsilon = innerAngle - outerAngle;
        float spotIntensity = clamp((theta - outerAngle) / epsilon, 0.0, 1.0);   

        result += blinn_phong(baseColor, specularColor, glossy, surfaceNormal, halfwayDirection, lightDirection, lightColor) * spotIntensity * attenuationSqrared;
        result += _spotLights[i].Color.w * baseColor * lightColor * attenuationLinear;
    }

    return result;
}


void main()
{
    vec4 baseColor = texture(BaseColorMap, _vertex.UV0);
    vec2 metallicRoughness = texture(MetallicRoughnessMap, _vertex.UV0).yz * MREO.xy;
    vec3 emmision = texture(EmissiveMap, _vertex.UV0).xyz * MREO.z;
    float occlusion = texture(OcclusionMap, _vertex.UV0).x * MREO.w;
    vec3 normal = normalize(texture(NormalMap, _vertex.UV0).xyz * 2.0 - 1.0);

    vec3 surfaceNormal = normalize(vec3(_vertex.NormalWorld));
    vec3 surfaceColor = emmision + evaluate_lights(baseColor.xyz, metallicRoughness.y, metallicRoughness.x, surfaceNormal);
    vec3 corrected = pow(surfaceColor, vec3(0.454545454545));

    OutputColor = vec4(corrected.xyz, baseColor.w);
}