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

vec3 random(vec3 p)
{
    p = fract(p);
    p *= WHITE_NOISE_SCALE;
	p = fract(p * vec3(.1031, .1030, .0973));
    p += dot(p, p.yxz+33.33);
    return normalize(fract((p.xxy + p.yxx)*p.zyx) - 0.5);
}

vec3 random_hemisphere(vec3 p, vec3 normal)
{
    return random(p) * sign(dot(p, normal));
}

vec2 DDADirection(mat4 projectionToView, vec2 uv, vec3 originWS, vec3 directionWS)
{
    // to view space
    vec4 projectionSpace = projectionToView * vec4(originWS + (directionWS * 0.1), 1.0);
    projectionSpace = (projectionSpace / projectionSpace.w) * 0.5 + 0.5;

    // SS direction
    vec2 pixelDirection = projectionSpace.xy - uv;

    // normalize to 1.0 on the dominant axis
    if (abs(pixelDirection.x) > abs(pixelDirection.y))
        return pixelDirection * (1.0 / abs(pixelDirection.x));
    else
        return pixelDirection * (1.0 / abs(pixelDirection.y));
}


uniform sampler2D LightMap;
uniform sampler2D DeferredPosition;
uniform sampler2D DeferredMRO;
uniform sampler2D DeferredNormalTexture;

uniform mat4 Projection;
uniform vec4 ViewPosition;

out vec4 OutputColor;


void main()
{
    OutputColor = vec4(vec3(0.0), 1.0);
    
    // General infos
    float margeScale = 5.0 + 5.0 * random(_vertexScreenQuad.UV0);
    vec2 texelSize = margeScale / textureSize(DeferredPosition, 0);
    vec3 surfaceNormalWS = normalize(texture(DeferredNormalTexture, _vertexScreenQuad.UV0).xyz);
    vec4 surfaceMREO = texture(DeferredMRO, _vertexScreenQuad.UV0);

    vec3 sampleOriginWS = texture(DeferredPosition, _vertexScreenQuad.UV0).xyz;
    vec2 sampleOriginSS = _vertexScreenQuad.UV0;
    vec3 viewDirectionWS = normalize(ViewPosition.xyz - sampleOriginWS);
    vec3 reflectionDirectionWS = reflect(-viewDirectionWS, surfaceNormalWS);
    vec3 sampleRaySeed = sampleOriginWS - vec3(_vertexScreenQuad.UV0, 0.0);// + vec3(fract(_time.Total));


    // Sample along random rays
    float rays = 200;
    float hits = 0;
    for (int i = 0; i < rays; i++)
    {
        sampleRaySeed = random(sampleRaySeed);
    
        // Gather direction infos
        vec3 sampleDirectionWS = normalize(mix(reflectionDirectionWS, random_hemisphere(sampleRaySeed, surfaceNormalWS), surfaceMREO.y));
        vec2 sampleDirectionSS = texelSize * DDADirection(Projection, _vertexScreenQuad.UV0, sampleOriginWS, sampleDirectionWS);
        float sampleDot = dot(viewDirectionWS, sampleDirectionWS);

        // Move along the ray unitl the end of the screen has been reached
        float hitDot = -1.0;
        float steps = 0.0;
        vec2 testPositionSS = sampleOriginSS + 3.0 * sampleDirectionSS;

        while(steps < 200.0 && testPositionSS.x > 0 && testPositionSS.x < 1 && testPositionSS.y > 0 && testPositionSS.y < 1)
        {
            steps++;

            vec3 testPositionWS = texture(DeferredPosition, testPositionSS).xyz;
            vec3 testDiff = testPositionWS - sampleOriginWS;
            vec3 testDirectionWS = normalize(testDiff);
            float testDot = dot(viewDirectionWS, testDirectionWS);

            // if the new dot is higher, a hit has been found
            if (testDot > sampleDot)
            {
                vec3 hitNormal = texture(DeferredNormalTexture, testPositionSS).xyz;
                float lambert = dot(surfaceNormalWS, testDirectionWS);
                float attenuation = 1.0 / (1.0 + (dot(testDiff, testDiff) * dot(testDiff, testDiff)));

                if (lambert > 0 && attenuation > 0.01 && dot(hitNormal, testDirectionWS) < 0)
                {
                    hits++;
                    OutputColor += lambert * attenuation * vec4(texture(LightMap, testPositionSS).xyz, 1.0);
                }

                break;
            }

            // move to the next pixel
            testPositionSS += sampleDirectionSS;
        }
    }

    OutputColor /= max(0.0001, hits);
    OutputColor *= 5.0; // multiplication only to see the results better
}