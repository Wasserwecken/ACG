﻿#version 430 core
in VertexScreenQuad
{
    vec4 Position;
    vec2 UV0;
} _vertexScreenQuad;

const float weight[5] = float[] (0.227027, 0.1945946, 0.1216216, 0.054054, 0.016216);

uniform sampler2D BufferMap;
uniform float Horizontal;

out vec4 OutputColor;

void main()
{             
    vec3 color = texture(BufferMap, _vertexScreenQuad.UV0).xyz;

    vec2 texelSize = 1.0 / textureSize(BufferMap, 0);
    vec3 result = color * weight[0];


    vec2 offset = vec2(0.0);
    if (Horizontal > 0.5)
        offset = vec2(texelSize.x, 0.0);
    else
        offset = vec2(0.0, texelSize.y);


    for(int i = 1; i < 5; ++i)
    {
        result += texture(BufferMap, _vertexScreenQuad.UV0 + (offset * i)).rgb * weight[i];
        result += texture(BufferMap, _vertexScreenQuad.UV0 - (offset * i)).rgb * weight[i];
    }

    OutputColor = vec4(result, 1.0);
}  