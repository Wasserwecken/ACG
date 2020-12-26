#version 330
in vec3 VertexNormalLocal;
in vec3 VertexNormalProjection;
in vec4 VertexColor;
in vec2 VertexUV;
in vec4 PositionLocal;
in vec4 PositionWorld;


uniform mat4 LocalToWord;
uniform mat4 LocalToProjection;
uniform float TimeTotal;
uniform float TimeDelta;
uniform vec3 ViewPosition;


uniform vec3 LightAmbientColor = vec3(0.1);
uniform vec3 LightDirectionalColor = vec3(0.5, 0.5, 0.4);
uniform vec3 LightDirectionalDirection = vec3(1.0, -1.0, 1.0);
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
    vec3 viewDirection = normalize(ViewPosition - PositionWorld.xyz);
    vec3 surfaceNormal = normalize(VertexNormalProjection);
    vec3 halfwayDirection = normalize(viewDirection + surfaceNormal);

    vec3 surfaceDiffuse = vec3(0.5);
    vec3 surfaceSpecular = vec3(0.9);
    
    vec3 surfaceColor = surfaceDiffuse * LightAmbientColor;
    surfaceColor += blinn_phong(surfaceDiffuse, surfaceSpecular, 32.0, surfaceNormal, halfwayDirection, -LightDirectionalDirection, LightDirectionalColor);

    vec3 corrected = pow(surfaceColor, vec3(0.454545454545));
    OutputColor = vec4(corrected, 1.0);
}