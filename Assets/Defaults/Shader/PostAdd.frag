#version 430 core
in VertexScreenQuad
{
    vec4 Position;
    vec2 UV0;
} _vertexScreenQuad;


uniform sampler2D TextureA;
uniform sampler2D TextureB;

out vec4 OutputColor;

void main()
{           
    OutputColor = texture(TextureA, _vertexScreenQuad.UV0);
    OutputColor += texture(TextureB, _vertexScreenQuad.UV0);
}