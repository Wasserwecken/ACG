#version 430 core
in VertexScreenQuad
{
    vec4 Position;
    vec2 UV0;
} _vertexScreenQuad;


uniform sampler2D BloomMap;
uniform sampler2D BufferMap;
out vec4 OutputColor;

void main()
{             
    OutputColor = texture(BufferMap, _vertexScreenQuad.UV0);
    OutputColor += vec4(texture(BloomMap, _vertexScreenQuad.UV0).xyz, 0.0);
}  