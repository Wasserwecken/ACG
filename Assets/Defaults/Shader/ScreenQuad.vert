#version 430 core
// INPUT VERTICIES
layout (location = 0) in vec3 BufferVertex;
layout (location = 3) in vec2 BufferUV0;

// SHADER OUTPUT
out VertexScreenQuad
{
    vec4 Position;
    vec2 UV0;
} _vertexScreenQuad;

// LOGIC
void main(void)
{
    _vertexScreenQuad.UV0 = BufferUV0;
    _vertexScreenQuad.Position = vec4(BufferVertex, 1.0);

    gl_Position = _vertexScreenQuad.Position;
}