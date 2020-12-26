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

uniform vec3 LightAmbientColor = vec3(0.1);
uniform vec3 LightDirectionalColor = vec3(0.5, 0.5, 0.4);
uniform vec3 LightDirectionalDirection = vec3(1.0, -1.0, 1.0);
uniform vec3 LightPointColor;
uniform vec3 LightPointPosition;
uniform vec3 LightPointDirection;

uniform sampler2D texture1;
uniform sampler2D texture2;
out vec4 OutputColor;

void main()
{
    vec3 surfaceNormal = normalize(VertexNormalProjection);

    vec3 diffuse = vec3(0.8);
    vec3 surfaceColor = diffuse * LightAmbientColor;

    surfaceColor += max(dot(surfaceNormal, -LightDirectionalDirection), 0.0) * LightDirectionalColor;


    OutputColor = vec4(surfaceColor, 1.0);
}