#version 430 core
in VertexScreenQuad
{
    vec4 Position;
    vec2 UV0;
} _vertexScreenQuad;


uniform sampler2D BufferMap;
uniform float Exposure;
out vec4 OutputColor;

void main()
{             
    vec3 color = texture(BufferMap, _vertexScreenQuad.UV0).xyz;

    vec3 mapped = vec3(1.0) - exp(-color * Exposure);
    mapped = pow(mapped, vec3(0.4545454545));
  
    OutputColor = vec4(mapped, 1.0);
}  