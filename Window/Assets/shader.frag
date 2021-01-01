#version 430 core
in VertexOut
{
    vec2 UV;
    vec4 NormalLocal;
    vec4 NormalWorld;
    vec4 NormalView;
    vec4 PositionLocal;
    vec4 PositionView;
    vec4 PositionWorld;
} _vertex;


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

layout (std430) buffer TimeData {
 float Total;
 float Delta;
 float TotalSin;
 float TotalSin01;
} _time;

layout (std430) buffer DirectionalLightData {
    DirectionalLight _directionalLights[];
};

layout (std430) buffer PointLightData {
    PointLight _pointLights[];
};

layout (std430) buffer SpotLightData {
    SpotLight _spotLights[];
};


layout (std140) uniform RenderSpaceData {
    mat4 LocalToWorld;
    mat4 LocalToView;
    mat4 LocalToProjection;
    mat4 LocalToWorldRotation;
    mat4 LocalToViewRotation;
    mat4 WorldToView;
    vec3 ViewPosition;
    vec3 ViewDirection;
} _space;


uniform sampler2D texture1;
uniform sampler2D texture2;
out vec4 OutputColor;


vec3 blinn_phong(vec3 surfaceDiffuse, vec3 surfaceSpecular, float smoothness, vec3 normal, vec3 halfway, vec3 lightDirection, vec3 lightColor)
{
    vec3 diffuse = surfaceDiffuse * max(dot(normal, lightDirection), 0.0);
    vec3 specular = surfaceSpecular * pow(max(dot(normal, halfway), 0.0), smoothness);

    return (diffuse + specular) * lightColor;
}

vec3 process_lights(vec3 surfaceDiffuse, vec3 surfaceSpecular, float smoothness, vec3 surfaceNormal)
{
    vec3 result = vec3(0.0);
    
    for(int i = 0; i < _directionalLights.length(); i++)
    {
        vec3 lightColor = _directionalLights[i].Color.xyz;
        vec3 lightDirection = -normalize(_directionalLights[i].Direction);
        vec3 halfwayDirection = normalize(lightDirection + _space.ViewDirection);

        result += blinn_phong(surfaceDiffuse, surfaceSpecular, smoothness, surfaceNormal, halfwayDirection, lightDirection, lightColor);
        result += _directionalLights[i].Color.w * surfaceDiffuse * lightColor;
    }
    
    for(int i = 0; i < _pointLights.length(); i++)
    {
        vec3 lightDiff = _pointLights[i].Position - _vertex.PositionWorld.xyz;
        vec3 lightColor = _pointLights[i].Color.xyz;
        float lightDistance = length(lightDiff);
        vec3 lightDirection = normalize(lightDiff);
        vec3 halfwayDirection = normalize(lightDirection + _space.ViewDirection);
        float attenuationSqrared = 1.0 / (1.0 + lightDistance * lightDistance);
        float attenuationLinear = 1.0 / (1.0 + lightDistance);

        result += blinn_phong(surfaceDiffuse, surfaceSpecular, smoothness, surfaceNormal, halfwayDirection, lightDirection, lightColor) * attenuationSqrared;
        result += _pointLights[i].Color.w * surfaceDiffuse * lightColor * attenuationLinear;
    }   
    
    for(int i = 0; i < _spotLights.length(); i++)
    {
        vec3 lightDiff = _spotLights[i].Position.xyz - _vertex.PositionWorld.xyz;
        vec3 lightColor = _spotLights[i].Color.xyz;
        float lightDistance = length(lightDiff);
        vec3 lightDirection = normalize(lightDiff);
        vec3 halfwayDirection = normalize(lightDirection + _space.ViewDirection);
        float attenuationSqrared = 1.0 / (1.0 + lightDistance * lightDistance);
        float attenuationLinear = 1.0 / (1.0 + lightDistance);

        float outerAngle = _spotLights[i].Position.w;
        float innerAngle = _spotLights[i].Direction.w;
        float theta = dot(lightDirection, -normalize(_spotLights[i].Direction.xyz));
        float epsilon = innerAngle - outerAngle;
        float spotIntensity = clamp((theta - outerAngle) / epsilon, 0.0, 1.0);   

        result += blinn_phong(surfaceDiffuse, surfaceSpecular, smoothness, surfaceNormal, halfwayDirection, lightDirection, lightColor) * spotIntensity * attenuationSqrared;
        result += _spotLights[i].Color.w * surfaceDiffuse * lightColor * attenuationLinear;
    }

    return result;
}

void main()
{
    vec3 surfaceNormal = normalize(vec3(_vertex.NormalView));
    vec3 surfaceDiffuse = vec3(0.4, 0.4, 0.4);
    vec3 surfaceSpecular = vec3(0.9, 0.7, 0.5);

    vec3 surfaceColor = process_lights(surfaceDiffuse, surfaceSpecular, 128.0, surfaceNormal);
    vec3 corrected = pow(surfaceColor, vec3(0.454545454545));
    
    OutputColor = vec4(corrected, 1.0);
}