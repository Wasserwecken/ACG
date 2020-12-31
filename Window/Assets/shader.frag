#version 430 core

in VertexOut
{
    vec2 UV;
    vec3 NormalLocal;
    vec3 NormalWorld;
    vec3 NormalView;
    vec4 PositionLocal;
    vec4 PositionView;
    vec4 PositionWorld;
} _vertex;

uniform vec3 LightAmbientColor;
uniform int LightDirectionalCount;
uniform vec3 LightDirectionalColor[3];
uniform vec3 LightDirectionalDirection[3];
uniform vec3 LightPointColor;
uniform vec3 LightPointPosition;
uniform vec3 LightPointDirection;

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
    vec3 viewDirection = vec3(0.0, 0.0, 1.0);
    vec3 surfaceNormal = normalize(_vertex.NormalView);
    vec3 halfwayDirection = normalize(viewDirection + surfaceNormal);

    vec3 surfaceDiffuse = vec3(0.5);
    vec3 surfaceSpecular = vec3(0.9, 0.7, 0.5);
    

    vec3 surfaceColor = surfaceDiffuse * LightAmbientColor;
    for(int i = 0; i <  LightDirectionalCount; i++)
    {
        vec3 lightDirection = -LightDirectionalDirection[i];
        vec3 lightColor = LightDirectionalColor[i];
        surfaceColor += blinn_phong(surfaceDiffuse, surfaceSpecular, 32.0, surfaceNormal, halfwayDirection, lightDirection, lightColor);
    }

    vec3 corrected = pow(surfaceColor, vec3(0.454545454545));
    OutputColor = vec4(_vertex.NormalWorld, 1.0);
}