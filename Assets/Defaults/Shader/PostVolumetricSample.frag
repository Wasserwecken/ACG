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




vec4 ToScreenSpace(mat4 projectionToView, vec3 postionWS)
{
    // to projection space
    vec4 screenSpace = projectionToView * vec4(postionWS, 1.0);
    
    // to NDC and SS
    return (screenSpace / screenSpace.w) * 0.5 + 0.5;
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
    vec3 viewPositionWS = ViewPosition.xyz;
    vec3 vertexPositionWS = texture(DeferredPosition, _vertexScreenQuad.UV0).xyz;
    vec3 viewDirectionWS = normalize(ViewPosition.xyz - vertexPositionWS);
    float marchDistance = distance(vertexPositionWS, ViewPosition.xyz);
    vec2 shadowDimensions = textureSize(ShadowMap, 0);

    for(int i = 0; i < _directionalLights.length(); i++)
    {
        // light & shadow information
        vec2 shadowAtlasStart = _directionalShadows[i].Area.xy;
        vec2 shadowAtlasSize = _directionalShadows[i].Area.zw;
        vec4 viewPositionSS = ToScreenSpace(_directionalShadows[i].Space, viewPositionWS);
        vec4 vertexPositionSS = ToScreenSpace(_directionalShadows[i].Space, vertexPositionWS);

        // DDA information
        vec2 ddaDirection = vertexPositionSS.xy - viewPositionSS.xy;
        vec2 ddaStep = DDAStep(ddaDirection, shadowDimensions * shadowAtlasSize) * 2;
        float ddaDistance = length(ddaDirection);
        float ddaStepDistance = length(ddaStep);

        // marching loop
        vec2 testPositionSS = viewPositionSS.xy + ddaStep;
        float testDistance = ddaStepDistance;
        
        while(testDistance < ddaDistance)
        {
            // sample information
            vec3 samplePositionWS = mix(viewPositionWS, vertexPositionWS, testDistance / ddaDistance);
            vec4 samplePositionSS = ToScreenSpace(_directionalShadows[i].Space, samplePositionWS);
            
            // depth test
            float sampleShadowDepth = texture(ShadowMap, shadowAtlasStart + testPositionSS * shadowAtlasSize).r;
            float isExposed = (samplePositionSS.z < sampleShadowDepth) ? 1.0 : 0.0;
            
            OutputColor.r += isExposed * ddaStepDistance * 2;

            testPositionSS += ddaStep;
            testDistance += ddaStepDistance;
        }

        OutputColor += vec4(0.0);
    }
}