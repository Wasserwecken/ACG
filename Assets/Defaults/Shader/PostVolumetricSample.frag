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




vec4 ToScreenSpace(mat4 projection, vec3 postionWS)
{
    // to projection space
    vec4 screenSpace = projection * vec4(postionWS, 1.0);
    
    // to NDC
    screenSpace.xyz /= screenSpace.w;

    //to SS
    screenSpace.xyz = (screenSpace.xyz  * 0.5) + 0.5;

    return screenSpace;
}

vec3 ToWorldSpace(mat4 inverseProjection, vec2 uv, float depth)
{
    // to NDC
    vec3 positonSS = vec3(uv, depth) * 2.0 - 1.0;
    
    // to WS
    return (inverseProjection * vec4(positonSS, 1.0)).xyz;
}

vec2 DDAStep(vec2 directionSS, vec2 resolution)
{
    // normalize to the dominant axis to hit a diffrent pixel each step
    if (abs(directionSS.x) > abs(directionSS.y))
        directionSS *= (1.0 / abs(directionSS.x));
    else
        directionSS *= (1.0 / abs(directionSS.y));

    // scale step to match the pixel size
    return directionSS / resolution;
}

float RayPlaneDistance(vec3 planeOrign, vec3 planeNormal, vec3 rayOrigin, vec3 rayDirection)
{
        float denom = dot(planeNormal, rayDirection);
        return dot(planeOrign - rayOrigin, planeNormal) / denom;
}


uniform sampler2D ShadowMap;
uniform sampler2D DeferredPosition;

uniform vec4 ViewPosition;
uniform int ClusterSize;

uniform vec3 VolumeColor;
uniform float VolumeDensity;
uniform float VolumeScattering;

out vec4 OutputColor;


void main()
{
    OutputColor = vec4(vec3(0.0), 1.0);

    // general information
    vec3 viewPositionWS = ViewPosition.xyz;
    vec3 vertexPositionWS = texture(DeferredPosition, _vertexScreenQuad.UV0).xyz;
    vec3 marchDifferenceWS = vertexPositionWS - viewPositionWS;
    float marchDistance = distance(vertexPositionWS, ViewPosition.xyz);
    vec2 shadowDimensions = textureSize(ShadowMap, 0);

    // volumetric light information
    vec3 volumetricLightColor = vec3(0.0);

    for(int i = 0; i < _directionalLights.length(); i++)
    {
        // light & shadow information
        vec2 shadowAtlasStart = _directionalShadows[i].Area.xy;
        vec2 shadowAtlasSize = _directionalShadows[i].Area.zw;
        vec3 lightDirectionWS = _directionalLights[i].Direction.xyz;
        vec3 lightColor = _directionalLights[i].Color.xyz;
        mat4 inverseProjection = inverse(_directionalShadows[i].Space);
        vec4 viewPositionSS = ToScreenSpace(_directionalShadows[i].Space, viewPositionWS);
        vec4 vertexPositionSS = ToScreenSpace(_directionalShadows[i].Space, vertexPositionWS);

        // DDA information
        vec2 ddaDirection = vertexPositionSS.xy - viewPositionSS.xy;
        vec2 ddaStep = DDAStep(ddaDirection, shadowDimensions * shadowAtlasSize) * 2;
        float ddaDistance = length(ddaDirection);
        float ddaStepDistance = length(ddaStep);

        // march test information
        vec3 helpingTestAxis = cross(_directionalLights[i].Direction.xyz, marchDifferenceWS);
        vec3 depthTestAxisWS = normalize(cross(marchDifferenceWS, helpingTestAxis));
        vec3 testPositionWS = vertexPositionWS;
        vec2 testPositionSS = viewPositionSS.xy + ddaStep;
        float testDistance = ddaStepDistance;

        // marching loop
        while(testDistance < ddaDistance)
        {
            // shadow map sampling
            float sampleDepth = texture(ShadowMap, shadowAtlasStart + testPositionSS * shadowAtlasSize).r;
            vec3 samplePositionWS = ToWorldSpace(inverseProjection, testPositionSS, sampleDepth);
            vec3 sampleDirectionWS = vertexPositionWS - samplePositionWS;

            // march WS information
            float marchDeviationWS = RayPlaneDistance(vertexPositionWS, depthTestAxisWS, samplePositionWS, lightDirectionWS);
            vec3 marchPositionWS = samplePositionWS + lightDirectionWS * marchDeviationWS;
            float isExposed = marchDeviationWS > 0 ? 1.0 : 0.0;
            float marchDistanceWS = distance(testPositionWS, marchPositionWS);

            // light contribution
            
            OutputColor.xyz += isExposed * marchDistanceWS * 0.01 * _directionalLights[i].Color.xyz;

            // next shadow texel
            testPositionWS = marchPositionWS;
            testPositionSS += ddaStep;
            testDistance += ddaStepDistance;
        }

        OutputColor += vec4(0.0);
    }
}