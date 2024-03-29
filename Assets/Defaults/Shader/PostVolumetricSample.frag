﻿// random algorithms sources
// https://github.com/Wasserwecken/glslLib/blob/master/lib/random.glsl
// https://www.shadertoy.com/view/4djSRW


#version 430 core
const float WHITE_NOISE_SCALE = 5461.5461;
const float PI = 3.14159265359;
const float PI4 = PI * 4.0;

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

// https://www.shadertoy.com/view/4ltGWl
float HenyeyGreenstein(float g, float costh)
{
    return (1.0 - g * g) / (PI4 * pow(1.0 + g * g - 2.0 * g * costh, 3.0/2.0));
}

float Schlick(float k, float costh)
{
    return (1.0 - k * k) / (PI4 * pow(1.0 - k * costh, 2.0));
}

uniform sampler2D ShadowMap;
uniform sampler2D DeferredPosition;

uniform vec4 ViewPosition;

uniform int ClusterSize;
uniform float MarchStepMaxSize;
uniform float MarchStepMaxCount;

uniform vec4 VolumeColor;
uniform float VolumeDensity;
uniform float VolumeScattering;

out vec4 OutputColor;


void main()
{
    OutputColor = vec4(0.0);

    // general information
    vec3 viewPositionWS = ViewPosition.xyz;
    vec3 vertexPositionWS = texture(DeferredPosition, _vertexScreenQuad.UV0).xyz;
    vec2 shadowDimensions = textureSize(ShadowMap, 0);

    // interleaved sampling
    ivec2 pixel = ivec2(gl_FragCoord.xy);
    float clusterElements = ClusterSize * ClusterSize;
    float clusterIndex = 1.0 + mod(pixel.x, ClusterSize) + ClusterSize * mod(pixel.y, ClusterSize);
    float clusterOffset = clusterIndex / clusterElements;

    // march information
    vec3 marchDifference = vertexPositionWS - viewPositionWS;
    vec3 marchDirection = normalize(marchDifference);
    float marchDistance = length(marchDifference);
    float marchStepCount = min(MarchStepMaxCount, trunc(marchDistance / MarchStepMaxSize)) / clusterElements;
    vec3 marchStep = marchDifference / marchStepCount;
    float marchStepDistance = marchDistance / marchStepCount;


    for(int i = 0; i < _directionalLights.length(); i++)
    {
        // light & shadow information
        vec2 shadowAtlasStart = _directionalShadows[i].Area.xy;
        vec2 shadowAtlasSize = _directionalShadows[i].Area.zw;
        vec3 lightColor = _directionalLights[i].Color.xyz;
        vec3 lightDirection = _directionalLights[i].Direction.xyz;

        // march start information
        vec3 testPositionWS = viewPositionWS + (marchStep * clusterOffset);
        float testDistance = marchStepDistance * clusterOffset;
                
        // marching loop
        while(testDistance < marchDistance)
        {
            // shadow map sampling
            vec4 samplePositionSS = ToScreenSpace(_directionalShadows[i].Space, testPositionWS);
            float sampleShadowDepth = texture(ShadowMap, shadowAtlasStart + samplePositionSS.xy * shadowAtlasSize).r;
            float isExposed = sampleShadowDepth < samplePositionSS.z ? 0.0 : 1.0;

            // light contribution
            if (isExposed > 0)
            {                
                vec3 incidenceLight = lightColor * VolumeDensity;
                vec3 radianceLight = VolumeColor.xyz;
                float phase = HenyeyGreenstein(VolumeScattering, dot(marchDirection, lightDirection));

                OutputColor.xyz += marchStepDistance * incidenceLight * radianceLight * phase;
            }

            // next test position
            testPositionWS += marchStep;
            testDistance += marchStepDistance;
        }
    }




    for(int i = 0; i < _spotLights.length(); i++)
    {
        // light & shadow information
        vec2 shadowAtlasStart = _spotShadows[i].Area.xy;
        vec2 shadowAtlasSize = _spotShadows[i].Area.zw;
        vec3 lightColor = _spotLights[i].Color.xyz;
            
        // march start information
        vec3 testPositionWS = viewPositionWS + (marchStep * clusterOffset);
        float testDistance = marchStepDistance * clusterOffset;
                
        // marching loop
        while(testDistance < marchDistance)
        {
            // light information
            vec3 lightDiff = _spotLights[i].Position.xyz - testPositionWS;
            vec3 lightDirection = normalize(lightDiff);
            float lightDistance = length(lightDiff);

            // range check
            if (lightDistance < _spotLights[i].Range.x)
            {
                // shadow map sampling
                vec4 samplePositionSS = ToScreenSpace(_spotShadows[i].Space, testPositionWS);
                float sampleShadowDepth = texture(ShadowMap, shadowAtlasStart + samplePositionSS.xy * shadowAtlasSize).r;
                float isExposed = sampleShadowDepth < samplePositionSS.z ? 0.0 : 1.0;

                // light contribution
                if (isExposed > 0 && samplePositionSS.x > 0 && samplePositionSS.x < 1.0 && samplePositionSS.y > 0 && samplePositionSS.y < 1.0 )
                {
                    float outerAngle = _spotLights[i].Position.w;
                    float innerAngle = _spotLights[i].Direction.w;
                    float epsilon = innerAngle - outerAngle;
                    float theta = dot(lightDirection, normalize(_spotLights[i].Direction.xyz));
                    float attenuation = 1 - clamp(lightDistance / _spotLights[i].Range.x, 0, 1);
                    attenuation *= attenuation * attenuation;
                    attenuation *= clamp((theta - outerAngle) / epsilon, 0.0, 1.0);
                    
                    vec3 incidenceLight = lightColor * attenuation * VolumeDensity;
                    vec3 radianceLight = VolumeColor.xyz;
                    float phase = HenyeyGreenstein(VolumeScattering, dot(marchDirection, lightDirection));

                    OutputColor.xyz += marchStepDistance * incidenceLight * radianceLight * phase;
                }
            }
            
            // next test position
            testPositionWS += marchStep;
            testDistance += marchStepDistance;
        }
    }
}