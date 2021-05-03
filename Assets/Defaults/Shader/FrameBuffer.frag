#version 430 core
in VertexScreenQuad
{
    vec4 Position;
    vec2 UV0;
} _vertexScreenQuad;

uniform sampler2D BufferMap;
out vec4 FragColor;

void main()
{    
    FragColor = texture(BufferMap, _vertexScreenQuad.UV0);
}