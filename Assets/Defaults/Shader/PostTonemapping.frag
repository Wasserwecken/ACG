#version 430 core
in VertexScreenQuad
{
    vec4 Position;
    vec2 UV0;
} _vertexScreenQuad;


uniform sampler2D BufferMap;
uniform float Exposure;
out vec4 OutputColor;


vec3 aces(vec3 x) {
  const float a = 2.51;
  const float b = 0.03;
  const float c = 2.43;
  const float d = 0.59;
  const float e = 0.14;
  return clamp((x * (a * x + b)) / (x * (c * x + d) + e), 0.0, 1.0);
}


void main()
{             
    vec3 color = Exposure * texture(BufferMap, _vertexScreenQuad.UV0).xyz;

    //aces
    float a = 2.51;
    float b = 0.03;
    float c = 2.43;
    float d = 0.59;
    float e = 0.14;
    vec3 mapped = clamp((color * (a * color + b)) / (color * (c * color + d) + e), 0.0, 1.0);

    OutputColor = vec4(pow(mapped, vec3(0.4545454545)), 1.0);
}  