#version 430 core
in VertexScreenQuad
{
    vec4 Position;
    vec2 UV0;
} _vertexScreenQuad;


uniform sampler2D AmbientOcclusionMap;
uniform sampler2D BufferMap;

out vec4 OutputColor;

void main()
{           
    OutputColor = texture(BufferMap, _vertexScreenQuad.UV0);
    OutputColor *= texture(AmbientOcclusionMap, _vertexScreenQuad.UV0);
}