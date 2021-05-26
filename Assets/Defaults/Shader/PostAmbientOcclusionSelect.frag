#version 430 core
const float SQRT205 = 0.70710678118;
const float SQRT2 = 1.41421356237;
const float SQRT305 = 0.86602540378;
const float SQRT3 = 1.73205080756;
const float PI025 = 0.78539816339;
const float PI05 = 1.57079632674;
const float PI = 3.14159265359;
const float PI2 = 6.28318530718;
const float RADTODEG = 57.295779513;
const float DEGTORAD = 0.01745329252;

in VertexScreenQuad
{
    vec4 Position;
    vec2 UV0;
} _vertexScreenQuad;


vec3 random_vec3(float p)
{
    p *= 5461.5461;
   vec3 p3 = fract(vec3(p) * vec3(.1031, .1030, .0973));
   p3 += dot(p3, p3.yzx+33.33);
   return fract((p3.xxy+p3.yzz)*p3.zyx) * 2.0 - 1.0; 
}

vec3 random_vec3(vec3 p)
{
    p = fract(p);
    p *= 5461.5461;
	p = fract(p * vec3(.1031, .1030, .0973));
    p += dot(p, p.yxz+33.33);
    return fract((p.xxy + p.yxx)*p.zyx) * 2.0 - 1.0;
}

uniform sampler2D DeferredMRO;
uniform sampler2D DeferredPosition;
uniform sampler2D DeferredNormalTexture;

uniform float Radius = 0.5;
uniform float Bias = 0.025;
uniform float Strength = 1.0;
uniform mat4 Projection;
uniform vec4 ViewPosition;

out vec4 OutputColor;

void main()
{
    vec3 position = texture(DeferredPosition, _vertexScreenQuad.UV0).xyz;
    vec3 normal = texture(DeferredNormalTexture, _vertexScreenQuad.UV0).xyz;
    float roughness = texture(DeferredMRO, _vertexScreenQuad.UV0).y;

    int samples = 16;
    float occlusion = 0.0;
    
    for(int i = 0; i < samples; ++i)
    {
        float scale = float(i) / samples;
        vec3 sampleOffset = Radius * random_vec3(position + random_vec3(float(i)));
        sampleOffset *= mix(0.1, 1.0, scale * scale);

        vec4 samplePosition = Projection * vec4(position + sampleOffset, 1.0);
        samplePosition.xyz /= samplePosition.w;
        samplePosition.xyz = samplePosition.xyz * 0.5 + 0.5;

        vec3 sampleDiff = ViewPosition.xyz - (position + sampleOffset);
        vec3 actualDiff = ViewPosition.xyz - texture(DeferredPosition, samplePosition.xy).xyz;
        
        float depthDiff = length(actualDiff) - length(sampleDiff);
        float rangeCheck = smoothstep(0.0, 1.0, (Radius) / abs(depthDiff));

        occlusion += ((depthDiff) < Bias ? 1.0 : 0.0) * rangeCheck;
    }
    
    occlusion = occlusion / samples;
    occlusion = mix(0.5, occlusion, roughness);
    occlusion = 1.0 - occlusion;

    OutputColor = vec4(vec3(occlusion), 1.0);
}  