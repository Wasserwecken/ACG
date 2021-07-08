﻿#version 430 core

const float WHITE_NOISE_SCALE = 5461.5461;


in VertexScreenQuad
{
    vec4 Position;
    vec2 UV0;
}
_vertexScreenQuad;

layout (std430) buffer ShaderTimeBlock {
    float Total;
    float TotalSin;
    float Frame;
    float Fixed;
} _time;

float random(vec2 p)
{
    p *= WHITE_NOISE_SCALE;
	vec3 p3  = fract(vec3(p.xyx) * .1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

vec3 random_hemisphere(vec3 p, vec3 normal)
{
    p = fract(p);
    p *= WHITE_NOISE_SCALE;
	p = fract(p * vec3(.1031, .1030, .0973));
    p += dot(p, p.yxz+33.33);
    
    // random direction
    vec3 direction = normalize(fract((p.xxy + p.yxx)*p.zyx) * 2.0 - 1.0);

    // orient to hemi sphere
    return direction * sign(dot(direction, normal));
}

vec2 DDAStep(vec3 direction, mat4 projection)
{
    // to SS
    vec2 result = (projection * vec4(direction, 0.0)).xy;

    // sclae to 1 on bigger axis
    if (abs(result.x) > abs(result.y))
        return result * (1.0 / abs(result.x));
    else
        return result * (1.0 / abs(result.y));
}


uniform sampler2D LightMap;
uniform sampler2D DeferredPosition;
uniform sampler2D DeferredNormalTexture;

uniform mat4 Projection;
uniform vec4 ViewPosition;
uniform vec4 ViewDirection;

out vec4 OutputColor;


void main()
{
    OutputColor = vec4(vec3(0.0), 1.0);
    
    // Preperations
    float texelScale = 5.0 + 10.0 * random(_vertexScreenQuad.UV0); // scale the texels step to avoid banding
    vec2 texelSize = texelScale / textureSize(DeferredPosition, 0); // scale for the DDA step, to avoid sampling every pixel
    vec3 normal = texture(DeferredNormalTexture, _vertexScreenQuad.UV0).xyz;

    // Determine origin
    vec3 sampleOriginWS = texture(DeferredPosition, _vertexScreenQuad.UV0).xyz;
    vec2 sampleOriginSS = _vertexScreenQuad.UV0;
    vec3 seed = sampleOriginWS - normal - vec3(_vertexScreenQuad.UV0, 0.0) + vec3(_time.TotalSin * 20.0);
    
    // Sample along random rays
    int rays = 3;
    for (int i = 0; i < rays; i++)
    {
        seed = (1.0 / seed) + 1.0; 

        // create random ray direction in WS and SS
        vec3 sampleDirectionWS = normalize(mix(random_hemisphere(seed, normal), normal, 0.5));
        vec2 sampleDirectionSS = texelSize * DDAStep(sampleDirectionWS, Projection);

        // check if something has been hit is done by dot. Therefore i can avoid to sample the depth buffer too.
        float sampleDot = dot(ViewDirection.xyz, sampleDirectionWS);

        // prepare for tracing
        vec2 testPositionSS = sampleOriginSS + sampleDirectionSS;
        vec3 hitPositon = vec3(testPositionSS, -1.0);
        vec3 traceColor = vec3(0.0);

        int steps = 0;
        int hits = 0;

        // check if max hits reached or screen has been left
        while(hits < 8 && testPositionSS.x > 0 && testPositionSS.x < 1 && testPositionSS.y > 0 && testPositionSS.y < 1)
        {
            steps++;

            // check where the ray is currently
            vec3 testPositionWS = texture(DeferredPosition, testPositionSS).xyz;
            vec3 testDiff = testPositionWS - sampleOriginWS;
            vec3 testDirectionWS = normalize(testDiff);
            float testDot = dot(normal.xyz, testDirectionWS);

            // If the test-dot is higher than the last one, that means the new hit is nearer to the camera
            // This point can also be used because the current test direction was not occluded to this point.
            if (testDot > hitPositon.z || testPositionWS == vec3(0.0))
            {
                hits++;
                hitPositon = vec3(testPositionSS, testDot);
                
                float attenuation = testPositionWS != vec3(0.0) ? 1.0 / (1.0 + (dot(testDiff, testDiff) * 0.25)) : 1.0;
                float lambert = max(0.0, dot(normal, testDirectionWS));
                traceColor += lambert * attenuation * texture(LightMap, testPositionSS).xyz;
            }
  
            // If the test-dot is higher is higher than the sample-dot, that means the ray finnaly hit something.
            if (testDot > sampleDot)
                break;

            // move along the pixels
            testPositionSS += sampleDirectionSS;
        }

        OutputColor += vec4(traceColor / float(hits), 1.0);
    }

    OutputColor /= rays;
}