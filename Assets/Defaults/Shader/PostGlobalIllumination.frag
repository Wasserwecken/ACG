#version 430 core
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
    p = normalize(fract((p.xxy + p.yxx)*p.zyx) - 0.5);

    return p * sign(dot(p, normal));
}

vec2 DDADirection(vec3 directionWS, mat4 projectionToView)
{
    // to view space
    vec4 viewSpace = projectionToView * vec4(directionWS, 1.0);
    vec2 pixelDirection = viewSpace.xy / viewSpace.w;

    // normalize to 1.0 on the dominant axis
    if (abs(pixelDirection.x) > abs(pixelDirection.y))
        return pixelDirection * (1.0 / abs(pixelDirection.x));
    else
        return pixelDirection * (1.0 / abs(pixelDirection.y));
}


uniform sampler2D LightMap;
uniform sampler2D DeferredPosition;
uniform sampler2D DeferredNormalTexture;

uniform mat4 Projection;
uniform vec4 ViewPosition;

out vec4 OutputColor;


void main()
{
    OutputColor = vec4(vec3(0.0), 1.0);
    
    // Preperations
    float texelStep = 1.0 + 0.0 * random(_vertexScreenQuad.UV0); // randomize texels steps to avoid banding
    vec2 texelSize = texelStep / textureSize(DeferredPosition, 0); // final DDA step length
    vec3 surfaceNormalWS = normalize(texture(DeferredNormalTexture, _vertexScreenQuad.UV0).xyz);

    vec3 sampleOriginWS = texture(DeferredPosition, _vertexScreenQuad.UV0).xyz;
    vec2 sampleOriginSS = _vertexScreenQuad.UV0;
    vec3 viewDirectionWS = normalize(ViewPosition.xyz - sampleOriginWS);
    vec3 sampleRaySeed = sampleOriginWS - vec3(_vertexScreenQuad.UV0, 0.0);// + vec3(fract(_time.Total));
    
    // Sample along random rays
    int rays = 1;
    int hits = 0;
    for (int i = 0; i < rays; i++)
    {
        sampleRaySeed = (1.0 / sampleRaySeed) + 1.0; 

        // Gather sample direction infos
        vec3 sampleDirectionWS = normalize(mix(random_hemisphere(sampleRaySeed, surfaceNormalWS), reflect(-viewDirectionWS, surfaceNormalWS), 1.0));
        vec2 sampleDirectionSS = texelSize * DDADirection(sampleDirectionWS, Projection);
        float sampleDot = dot(viewDirectionWS, sampleDirectionWS);

        // Move along the ray unitl the end of the screen has been reached
        float hitDot = -1.0;
        vec2 testPositionSS = sampleOriginSS + sampleDirectionSS;
        
        while(testPositionSS.x > 0 && testPositionSS.x < 1 && testPositionSS.y > 0 && testPositionSS.y < 1)
        {
            vec3 testPositionWS = texture(DeferredPosition, testPositionSS).xyz;
            vec3 testDiff = testPositionWS - sampleOriginWS;
            vec3 testDirectionWS = normalize(testDiff);
            float testDot = dot(viewDirectionWS, testDirectionWS);

            if (testDot > hitDot)
                hitDot = testDot;

            if (testPositionWS == vec3(0.0)) // vec3(0.0) indicates a skybox hit, will handled properly later
                continue;

            if (testDot > sampleDot)
            {
                vec3 hitNormal = texture(DeferredNormalTexture, testPositionSS).xyz;
                if (dot(hitNormal, -testDirectionWS) > 0)
                {
                    hits++;
                    
                    float lambert = dot(surfaceNormalWS, testDirectionWS);
                    float attenuation = 1.0 / (1.0 + (dot(testDiff, testDiff) * dot(testDiff, testDiff)));
                    OutputColor += vec4(texture(LightMap, testPositionSS).xyz, 1.0);
                }

                break;
            }

            // move along the pixels
            testPositionSS += sampleDirectionSS;
        }
    }

    OutputColor /= max(0.00001, float(hits));
    OutputColor *= 5.0;
}