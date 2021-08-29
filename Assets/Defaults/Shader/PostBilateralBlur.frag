#version 430 core
#define MSIZE 5

in VertexScreenQuad
{
    vec4 Position;
    vec2 UV0;
} _vertexScreenQuad;

//const float weight[MSIZE] = float[] (0.227027, 0.1945946, 0.1216216, 0.054054, 0.016216);
const float weight[MSIZE] = float[] (0.398942280401, 0.241970724519, 0.0539909665132, 0.00443184841194, 0.000133830225765);

uniform sampler2D BufferMap;
uniform sampler2D DeferredDepth;
uniform float Horizontal;
uniform vec4 Clipping;

out vec4 OutputColor;

float LinearizeDepth(float depth,float zNear,float zFar)
{
    return zNear * zFar / (zFar + depth * (zNear - zFar));
}

float normpdf3(in vec3 v, in float sigma)
{
	return 0.39894*exp(-0.5*dot(v,v)/(sigma*sigma))/sigma;
}

void main()
{             
    vec3 color = texture(BufferMap, _vertexScreenQuad.UV0).xyz;
    float depth = LinearizeDepth(texture(DeferredDepth, _vertexScreenQuad.UV0).x, Clipping.x, Clipping.y);


    vec2 texelSize = 1.0 / textureSize(BufferMap, 0);
    vec2 direction = vec2(0.0);
    if (Horizontal > 0.5)
        direction = vec2(texelSize.x, 0.0);
    else
        direction = vec2(0.0, texelSize.y);


    vec3 resultColor = color.xyz * weight[0];
    float resultWeight = weight[0];


    for(int i = -MSIZE + 1; i < MSIZE; ++i)
    {
        if (i == 0) continue;
        vec2 offset = direction * i;

        vec3 sampleColor = texture(BufferMap, _vertexScreenQuad.UV0 + offset).xyz;
        float sampleDepth = LinearizeDepth(texture(DeferredDepth, _vertexScreenQuad.UV0 + offset).x, Clipping.x, Clipping.y);

        float depthDifference = (depth - sampleDepth) * 5.0;
        float factor = exp(-depthDifference * depthDifference);

        resultColor += factor * weight[i] * sampleColor;
        resultWeight += factor * weight[i];
    }

    OutputColor = vec4(resultColor / resultWeight, 1.0);
}  