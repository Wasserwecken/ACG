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
 vec3 Color;
 vec3 Direction;
};

struct PointLight
{
 vec3 Color;
 vec3 Position;
};

layout (std430) buffer DirectionalLightData {
    DirectionalLight _directionalLights[];
};

uniform vec3 LightAmbientColor;

uniform sampler2D texture1;
uniform sampler2D texture2;
out vec4 OutputColor;


vec3 blinn_phong(vec3 surfaceDiffuse, vec3 surfaceSpecular, float smoothness, vec3 normal, vec3 halfway, vec3 lightDirection, vec3 lightColor)
{
    vec3 diffuse = surfaceDiffuse * max(dot(normal, lightDirection), 0.0);
    vec3 specular = surfaceSpecular * pow(max(dot(normal, halfway), 0.0), smoothness);

    return (diffuse + specular) * lightColor;
}


void main()
{
    vec3 surfaceNormal = normalize(vec3(_vertex.NormalView));
    vec3 viewDirection = vec3(0.0, 0.0, 1.0);

    vec3 surfaceDiffuse = vec3(0.5);
    vec3 surfaceSpecular = vec3(0.9, 0.7, 0.5);

    vec3 surfaceColor = surfaceDiffuse * LightAmbientColor;
    for(int i = 0; i <  _directionalLights.length(); i++)
    {
        vec3 lightColor = _directionalLights[i].Color;
        vec3 lightDirection = -_directionalLights[i].Direction;
        vec3 halfwayDirection = normalize(lightDirection + viewDirection);
        surfaceColor += blinn_phong(surfaceDiffuse, surfaceSpecular, 128.0, surfaceNormal, halfwayDirection, lightDirection, lightColor);
    }

    vec3 corrected = pow(surfaceColor, vec3(0.454545454545));
    OutputColor = vec4(corrected, 1.0);
}