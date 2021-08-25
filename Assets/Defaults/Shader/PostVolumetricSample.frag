// random algorithms sources
// https://github.com/Wasserwecken/glslLib/blob/master/lib/random.glsl
// https://www.shadertoy.com/view/4djSRW



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


// INPUT GLOBAL UNIFORMS LIGHT
struct DirectionalLight
{
    vec4 Color;
    vec4 Direction;
};

struct DirectionalShadow
{
    vec4 Strength;
    vec4 Area;
    mat4 Space;
};

struct PointLight
{
    vec4 Color;
    vec4 Position;
};

struct PointShadow
{
    vec4 Strength;
    vec4 Area;
};

struct SpotLight
{
    vec4 Color;
    vec4 Position;
    vec4 Direction;
    vec4 Range;
};

struct SpotShadow
{
    vec4 Strength;
    vec4 Area;
    mat4 Space;
};


layout (std430) buffer ShaderDirectionalLightBlock {
    DirectionalLight _directionalLights[];
};

layout (std430) buffer ShaderDirectionalShadowBlock {
    DirectionalShadow _directionalShadows[];
};

layout (std430) buffer ShaderPointLightBlock {
    PointLight _pointLights[];
};

layout (std430) buffer ShaderPointShadowBlock {
    PointShadow _pointShadows[];
};

layout (std430) buffer ShaderSpotLightBlock {
    SpotLight _spotLights[];
};

layout (std430) buffer ShaderSpotShadowBlock {
    SpotShadow _spotShadows[];
};


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

vec4 ToScreenSpace(mat4 projectionToView, vec3 postionWS)
{
    // projection
    vec4 projection = projectionToView * vec4(postionWS, 1.0);
    
    // to standard space
    return (projection / projection.w) * 0.5 + 0.5;
}

void DDA(vec2 startSS, vec2 endSS, out vec2 ddaDirection, out float ddaDistance)
{
    // direction
    ddaDirection = endSS - startSS;
    ddaDistance = length(ddaDirection);
    
    // normalize to 1.0 on the dominant axis
    if (abs(ddaDirection.x) > abs(ddaDirection.y))
        ddaDirection *= (1.0 / abs(ddaDirection.x));
    else
        ddaDirection *= (1.0 / abs(ddaDirection.y));
}


uniform sampler2D ShadowMap;
uniform sampler2D DeferredPosition;

uniform vec4 ViewPosition;
uniform float MaxMarchingSteps = 32.0;
uniform float DDAStepsBase = 5.0;
uniform float DDAStepsRandom = 5.0;

out vec4 OutputColor;


void main()
{
    OutputColor = vec4(vec3(0.0), 1.0);

    // general information
    vec2 shadowDimensions = textureSize(ShadowMap, 0);
    vec3 vertexPositionWS = texture(DeferredPosition, _vertexScreenQuad.UV0).xyz;
    vec3 viewDirectionWS = normalize(ViewPosition.xyz - vertexPositionWS);
    float marchDistance = distance(vertexPositionWS, ViewPosition.xyz);

    for(int i = 0; i < _directionalLights.length(); i++)
    {
        // directional information
        vec2 shadowAtlasStart = _directionalShadows[i].Area.xy;
        vec2 shadowAtlasSize = _directionalShadows[i].Area.zw;
        vec4 viewPositionSS = ToScreenSpace(_directionalShadows[i].Space, ViewPosition.xyz);
        vec4 vertexPositionSS = ToScreenSpace(_directionalShadows[i].Space, vertexPositionWS);

        // shadow depth
        float vertexDepth = texture(ShadowMap, shadowAtlasStart + vertexPositionSS.xy * shadowAtlasSize).r;
        float viewDepth = texture(ShadowMap, shadowAtlasStart + viewPositionSS.xy * shadowAtlasSize).r;

        // DDA information
        vec2 ddaDirection;
        float ddaDistance;
        DDA(viewPositionSS.xy, vertexPositionSS.xy, ddaDirection, ddaDistance);
        vec2 ddaStep = ddaDirection / (shadowDimensions * shadowAtlasSize);
        float ddaStepDistance = length(ddaStep);

        // ray marching loop
        float testDistance = ddaStepDistance;
        vec2 testPositionSS = viewPositionSS.xy + ddaStep;
        while(testDistance < ddaDistance)
        {
            float sampleDepth = texture(ShadowMap, shadowAtlasStart + testPositionSS * shadowAtlasSize).r;
            vec3 samplePositionWS = mix(ViewPosition.xyz, vertexPositionWS, ddaDistance / testDistance);
            vec4 samplePositionSS = ToScreenSpace(_directionalShadows[i].Space, samplePositionWS);
            float isExposed = (samplePositionSS.z > sampleDepth) ? 0.0 : 1.0;
            
            OutputColor.r += ddaStepDistance * isExposed;

            testDistance += ddaStepDistance;
            testPositionSS += ddaStep;
        }

        OutputColor += vec4(0.0);
    }

//
//    // general information
//    vec2 textureDimensions = textureSize(DeferredPosition, 0);
//    vec2 ddaStepScale = (DDAStepsBase + DDAStepsRandom * random(_vertexScreenQuad.UV0)) / textureDimensions;
//    vec3 sampleRaySeed = originPositionWS + vec3(originPositionSS, 0.0) + vec3(fract(_time.Total));
//
//    // Loop through rays
//    float hitCount = 0;
//    for (int i = 0; i < RayCount; i++)
//    {
//        sampleRaySeed = random(sampleRaySeed);
//    
//        // Create ray information
//        vec3 sampleDirectionWS = random_hemisphere(sampleRaySeed, originNormalWS);
//        vec2 sampleDirectionSS = ddaStepScale * DDADirection(Projection, originPositionSS, originPositionWS, sampleDirectionWS);
//        vec2 testPositionSS = originPositionSS + sampleDirectionSS;
//        float sampleTargetDot = dot(viewDirectionWS, sampleDirectionWS);
//
//        // Ray marching loop
//        float closestHitDot = -1.0;
//        float stepCount = 0.0;
//        
//        while(stepCount < MaxMarchingSteps && testPositionSS.x > 0 && testPositionSS.x < 1 && testPositionSS.y > 0 && testPositionSS.y < 1)
//        {
//            stepCount++;
//
//            // Gather information about the test / ray location
//            vec3 testPositionWS = texture(DeferredPosition, testPositionSS).xyz;
//            vec3 testDifference = testPositionWS - originPositionWS;
//            vec3 testDirectionWS = normalize(testDifference);
//            float testDot = dot(viewDirectionWS, testDirectionWS);
//
//            // If the new dot is higher, than the closest one, the point is not occluded.
//            if (testDot > closestHitDot)
//            {
//                closestHitDot = testDot;
//
//                // Check if the hit has viable light information.
//                // If the hit is "behind" the origin, it will not contribute.
//                float lambert = dot(originNormalWS, testDirectionWS);
//                if (lambert > 0)
//                {
//                    // If the hit is "facing away" from the origin, it will not contribute.
//                    vec3 hitNormal = -testDirectionWS;// texture(DeferredNormalSurface, testPositionSS).xyz;
//                    if (dot(hitNormal, testDirectionWS) < 0)
//                    {
//                        hitCount++;
//                        
//                        float distanceSquared = dot(testDifference, testDifference);
//                        float attenuation = 1.0 / (1.0 + (distanceSquared * distanceSquared));
//                        vec3 light = vec3(1.0); // vec4(texture(LightMap, testPositionSS).xyz, 1.0).xyz;
//                        
//                        OutputColor.xyz += lambert * attenuation * light;
//                    }
//                }
//            }
//
//            // if the testDot is grater than the target dot, the ray has now hit something in the scene.
//            if (testDot > sampleTargetDot)
//                break;
//
//            // move to the next pixel
//            testPositionSS += sampleDirectionSS;
//        }
//    }
//
//    // average out the results of the gathered light information. (monte carlo)
//    OutputColor /= max(0.0001, hitCount);
//
//    OutputColor *= 5.0; // multiplication only to see the results better
}