#version 430 core

in VertexScreenQuad
{
    vec4 Position;
    vec2 UV0;
}
_vertexScreenQuad;

uniform sampler2D TraceMap;
uniform int BufferLength;
uniform int IndexX;
uniform int IndexY;

out vec4 OutputColor;


void main()
{
    ivec2 pixel = ivec2(gl_FragCoord.xy);

    if (mod(pixel.x + IndexX, BufferLength) == 0 && mod(pixel.y + IndexY, BufferLength) == 0)
        OutputColor = texture(TraceMap, _vertexScreenQuad.UV0);
    else
        discard;
}