#version 430 core
in VertexScreenQuad
{
    vec4 Position;
    vec2 UV0;
} _vertexScreenQuad;


uniform sampler2D BufferMap;
uniform float ThresholdStart;
uniform float ThresholdEnd;
out vec4 OutputColor;

void main()
{             
    vec3 color = texture(BufferMap, _vertexScreenQuad.UV0).xyz;
    float brightness = dot(color, vec3(0.2126, 0.7152, 0.0722));

    OutputColor = vec4(smoothstep(ThresholdStart, ThresholdEnd, brightness) * color, 1.0);
}