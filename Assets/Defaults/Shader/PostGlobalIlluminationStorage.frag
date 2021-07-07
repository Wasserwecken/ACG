#version 430 core

in VertexScreenQuad
{
    vec4 Position;
    vec2 UV0;
}
_vertexScreenQuad;


uniform sampler2D TraceMap;

out vec4 OutputColor;


void main()
{
    OutputColor = vec4(vec3(0.0), 1.0);
}