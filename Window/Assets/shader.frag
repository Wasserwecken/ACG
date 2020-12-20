﻿#version 330
in vec3 VertexNormal;
in vec4 VertexColor;
in vec2 VertexUV;

in vec4 LocalPosition;
in vec4 WorldPosition;


uniform float TimeTotal;
uniform float TimeDelta;

uniform mat4 WorldSpace;
uniform mat4 ViewSpace;
uniform mat4 ProjectionSpace;

out vec4 outputColor;


uniform sampler2D texture1;
uniform sampler2D texture2;



void main()
{
    float offset = sin(TimeTotal * 0.5) * 0.5;
    vec4 texCol1 = texture(texture1, VertexUV + vec2(offset));
    vec4 texCol2 = texture(texture2, VertexUV - vec2(offset));

    outputColor = mix(texCol1, texCol2, texCol2.w * 0.5);
}