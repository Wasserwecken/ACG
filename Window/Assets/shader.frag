#version 330
in vec3 VertexNormal;
in vec4 VertexColor;
in vec2 VertexUV;

in vec4 LocalPosition;
in vec4 WorldPosition;


uniform float TimeTotal;
uniform float TimeDelta;

uniform mat4 LocalToWord;
uniform mat4 LocalToProjection;

out vec4 OutputColor;


uniform sampler2D texture1;
uniform sampler2D texture2;


void main()
{
    float offset = sin(TimeTotal * 0.2) * 0.5;
    vec4 texCol1 = texture(texture1, VertexUV + vec2(offset));
    vec4 texCol2 = texture(texture2, VertexUV - vec2(offset));

    OutputColor = mix(texCol1, texCol2, texCol2.w * 0.5);
}