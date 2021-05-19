#version 430 core
// CONSTANTS
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

const vec2 VOGELDISKPOINTS[32] = vec2[32]
(
    vec2(+0.125000000, +0.000000000),
    vec2(-0.159645036, +0.146247968),
    vec2(+0.024436183, -0.278438270),
    vec2(+0.201222390, +0.262458682),
    vec2(-0.369267583, -0.065318100),
    vec2(+0.349802434, -0.222515762),
    vec2(-0.117001638, +0.435242027),
    vec2(-0.223136023, -0.429633945),
    vec2(+0.484115213, +0.176797718),
    vec2(-0.503641009, +0.207896024),
    vec2(+0.242788091, -0.518824577),
    vec2(+0.179414541, +0.572001278),
    vec2(-0.540757656, -0.313378632),
    vec2(+0.634369314, -0.139465556),
    vec2(-0.387144893, +0.550675809),
    vec2(-0.089440741, -0.690199494),
    vec2(+0.549072444, +0.462757528),
    vec2(-0.738878369, +0.030555887),
    vec2(+0.538954556, -0.536332965),
    vec2(-0.036057420, +0.779791534),
    vec2(-0.512818038, -0.614526451),
    vec2(+0.812359691, +0.109301291),
    vec2(-0.688310385, +0.478908986),
    vec2(+0.188085750, -0.836061537),
    vec2(+0.435036361, +0.759189367),
    vec2(-0.850449383, -0.271312892),
    vec2(+0.826101065, -0.381683379),
    vec2(-0.357885152, +0.855156898),
    vec2(-0.319410414, -0.888032675),
    vec2(+0.849910140, +0.446685374),
    vec2(-0.944033802, +0.248847470),
    vec2(+0.536593318, -0.834531426)
);

const int SHADOWSAMPLECOUNT = 8;
const float SHADOWSAMPLECOUNTSQRT = sqrt(SHADOWSAMPLECOUNT);
const float VOGELPOINTRATIO = 32 / SHADOWSAMPLECOUNT;

// ENVIRONMENT
uniform samplerCube SkyboxMap;
uniform sampler2D ReflectionMap;
uniform sampler2D ShadowMap;

// INPUT GLOBAL UNIFORMS
layout (std430) buffer ShaderDeferredViewBlock {
    vec4 ViewPosition;
    vec4 ViewPort;
    vec2 Resolution;
} _viewDeferred;

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

layout (std430) buffer ShaderDirectionalLightBlock {
    DirectionalLight _directionalLights[];
};

layout (std430) buffer ShaderDirectionalShadowBlock {
    DirectionalShadow _directionalShadows[];
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

layout (std430) buffer ShaderPointLightBlock {
    PointLight _pointLights[];
};

layout (std430) buffer ShaderPointShadowBlock {
    PointShadow _pointShadows[];
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

layout (std430) buffer ShaderSpotLightBlock {
    SpotLight _spotLights[];
};

layout (std430) buffer ShaderSpotShadowBlock {
    SpotShadow _spotShadows[];
};

struct ShaderReflectionProbe
{
    vec4 Position;
    vec4 Area;
};

layout (std430) buffer ShaderReflectionBlock {
    ShaderReflectionProbe _reflectionProbes[];
};

// LOGIC

vec3 VogelConePoint(int index, int samples, float phi, float angle)
{
    float goldenAngle = 2.399963229;
    float theta = index * goldenAngle + phi;
    float z = 1.0 - ((1.0 - cos(angle)) * sqrt(index + 0.5) / SHADOWSAMPLECOUNTSQRT);
    float r = sqrt(1 - (z * z));
    return vec3(r * cos(theta), r * sin(theta), z);
}

float ShadowHash(vec2 point)
{
  vec3 seed = vec3(0.06711056f, 0.00583715f, 52.9829189f);
  return fract(seed.z * fract(dot(point, seed.xy)));
}

vec3 blinn_phong(vec3 surfaceDiffuse, vec3 surfaceSpecular, float roughness, vec3 normal, vec3 halfway, vec3 lightDirection, vec3 lightColor)
{
    roughness = 1.0 - roughness;
    roughness = roughness * roughness;

    float glossy = mix(128.0, 1.0, roughness);
    float luminance = max(dot(normal, lightDirection), 0.0);
    vec3 specular = roughness * pow(max(dot(normal, halfway), 0.0), glossy) * surfaceSpecular;

    return (surfaceDiffuse + specular) * lightColor * luminance;
}

float LinearizeDepth(float depth, float near, float far)
{
    float z = depth * 2.0 - 1.0; // back to NDC 
    return (2.0 * near * far) / (far + near - z * (far - near));	
}

float SamplePointShadowDepth(vec3 direction, vec2 atlasStart, vec2 atlasSize, vec2 clipping)
{
	vec3 directionAbs = abs(direction);
	float ma;
	vec2 faceUV;
    vec2 faceId;
    
	if(directionAbs.z >= directionAbs.x && directionAbs.z >= directionAbs.y)
	{
        faceId.x = 0.0;
		faceId.y = direction.z < 0.0 ? 0.0 : 1.0;
		ma = 0.5 / directionAbs.z;
		faceUV = vec2(direction.z > 0.0 ? -direction.x : direction.x, -direction.y);
	}
	else if(directionAbs.y >= directionAbs.x)
	{
        faceId.x = 1.0;
		faceId.y = direction.y < 0.0 ? 0.0 : 1.0;
		ma = 0.5 / directionAbs.y;
		faceUV = vec2(direction.z, direction.y < 0.0 ? -direction.x : direction.x);
	}
	else
	{
        faceId.x = 2.0;
		faceId.y = direction.x < 0.0 ? 0.0 : 1.0;
		ma = 0.5 / directionAbs.x;
		faceUV = vec2(direction.x > 0.0 ? direction.z : -direction.z, -direction.y);
	}

    faceUV = faceUV * ma + 0.5;

    float depthCorrection = length(vec3(faceUV * 2.0 - 1.0, 1.0));
    vec2 shadowUV = atlasStart + ((faceUV + faceId) / vec2(3.0, 2.0)) * atlasSize;
    return LinearizeDepth(texture(ShadowMap, shadowUV).r, clipping.x, clipping.y) * depthCorrection;
}

// FRAGMENT STRUCTS
struct FragmentMaterial
{
  vec3 Albedo;
  vec3 MRO;
  vec3 Emmision;
  vec3 Normal;
} _fragmentMaterial;

struct FragmentSurface
{
    vec4 Position;
    vec3 ViewDiff;
    vec3 ViewDirection;
    float ViewLength;
    vec3 NormalWorld;
    mat3 TangentSpace;
} _fragmentSurface;

// INPUT SPECIFIC UNIFORMS
uniform sampler2D DeferredPosition;
uniform sampler2D DeferredAlbedo;
uniform sampler2D DeferredNormalSurface;
uniform sampler2D DeferredNormalTexture;
uniform sampler2D DeferredMRO;
uniform sampler2D DeferredEmission;

// SHADER OUTPUT
out vec4 OutputColor;

// MAIN
void main()
{    
    vec2 screenUV = (gl_FragCoord.xy / _viewDeferred.Resolution);
   
    _fragmentSurface.Position = texture(DeferredPosition, screenUV);
    _fragmentSurface.ViewDiff = _fragmentSurface.Position.xyz - _viewDeferred.ViewPosition.xyz;
    _fragmentSurface.ViewDirection = normalize(_fragmentSurface.ViewDiff);
    _fragmentSurface.ViewLength = length(_fragmentSurface.ViewDiff);
    _fragmentSurface.NormalWorld = texture(DeferredNormalSurface, screenUV).xyz;

    _fragmentMaterial.Albedo = pow(texture(DeferredAlbedo, screenUV).xyz, vec3(2.2));
    _fragmentMaterial.MRO = texture(DeferredMRO, screenUV).xyz;
    _fragmentMaterial.Emmision = texture(DeferredEmission, screenUV).xyz;
    _fragmentMaterial.Normal = texture(DeferredNormalTexture, screenUV).xyz;

        
    vec3 reflectionDirection = reflect(_fragmentSurface.ViewDirection, _fragmentMaterial.Normal);
    vec3 reflectionColor = texture(SkyboxMap, reflectionDirection).xyz;
    OutputColor = vec4(_fragmentMaterial.Emmision, 1.0);
    OutputColor.xyz = reflectionColor * _fragmentMaterial.Albedo * _fragmentMaterial.MRO.x * (1.0 - _fragmentMaterial.MRO.y);

        
    float shadowSampleSeed = ShadowHash(gl_FragCoord.xy);
    vec2 ShadowMapPixels = vec2(textureSize(ShadowMap, 0));
   
    
    for(int i = 0; i < _directionalLights.length(); i++)
    {
        vec3 lightColor = _directionalLights[i].Color.xyz;
        vec3 lightDirection = _directionalLights[i].Direction.xyz;
        vec3 halfwayDirection = normalize(lightDirection + _fragmentSurface.ViewDirection);
        vec3 diffuseColor = _fragmentMaterial.Albedo * (1.0 - _fragmentMaterial.MRO.x);
        vec3 specularColor = mix(lightColor, _fragmentMaterial.Albedo * lightColor, _fragmentMaterial.MRO.x);
        vec3 surfaceColor = blinn_phong(diffuseColor, specularColor, _fragmentMaterial.MRO.y, _fragmentMaterial.Normal, halfwayDirection, lightDirection, lightColor);

        if (_directionalShadows[i].Strength.x > 0.001)
        {
            float shadowWidth = _directionalShadows[i].Strength.w;
            vec2 shadowClipping = _directionalShadows[i].Strength.yz;
            vec2 shadowAtlasStart = _directionalShadows[i].Area.xy;
            vec2 shadowAtlasSize = _directionalShadows[i].Area.zw;

            vec4 shadowWorldPosition = _directionalShadows[i].Space * _fragmentSurface.Position;
            vec3 shadowScreenPosition = (shadowWorldPosition.xyz / shadowWorldPosition.w) * 0.5 + 0.5;
            float shadowBias = 0.003 * (1.0 - dot(_fragmentSurface.NormalWorld, lightDirection)) + 0.001;
            float penumbra = 0.1 * (shadowWidth / ShadowMapPixels.x);
            float occluderCount = 0.0;

            float sampleAngle = shadowSampleSeed * PI;
            float s = sin(sampleAngle);
	        float c = cos(sampleAngle);
	        mat2 sampleRotation = mat2(c, -s, s, c);
            
            for (int i = 0; i < SHADOWSAMPLECOUNT; i++)
            {
                vec2 vogelPoint = sampleRotation * VOGELDISKPOINTS[i] * VOGELPOINTRATIO;
                vec2 sampleUV = shadowScreenPosition.xy + vogelPoint * penumbra;
                float sampleDepth = texture(ShadowMap, shadowAtlasStart + sampleUV * shadowAtlasSize).r;

                occluderCount += shadowScreenPosition.z > (sampleDepth + shadowBias) ? 1 : 0;
            }

            surfaceColor *= 1 - (occluderCount / float(SHADOWSAMPLECOUNT));
        }

        OutputColor.xyz += surfaceColor + _directionalLights[i].Color.w * _fragmentMaterial.Albedo * lightColor;
    }
    
    for(int i = 0; i < _pointLights.length(); i++)
    {
        vec3 lightDiff = _pointLights[i].Position.xyz - _fragmentSurface.Position.xyz;
        vec3 lightColor = _pointLights[i].Color.xyz;
        float lightDistance = length(lightDiff);

        if (_pointLights[i].Position.w > lightDistance)
        {
            vec3 lightDirection = normalize(lightDiff);
            vec3 halfwayDirection = normalize(lightDirection + -_fragmentSurface.ViewDirection);
            float attenuation = pow(1 - clamp(lightDistance / _pointLights[i].Position.w, 0, 1), 3.0);
            vec3 diffuseColor = _fragmentMaterial.Albedo * (1.0 - _fragmentMaterial.MRO.x);
            vec3 specularColor = mix(lightColor, _fragmentMaterial.Albedo * lightColor, _fragmentMaterial.MRO.x);
            vec3 surfaceColor = blinn_phong(diffuseColor, specularColor, _fragmentMaterial.MRO.y, _fragmentMaterial.Normal, halfwayDirection, lightDirection, lightColor) * attenuation;

            if (_pointShadows[i].Strength.x > 0.001)
            {
                vec2 shadowClipping = vec2(_pointShadows[i].Strength.y, _pointLights[i].Position.w);
                vec2 shadowAtlasStart = _pointShadows[i].Area.xy;
                vec2 shadowAtlasSize = _pointShadows[i].Area.wz;
                float penumbra = 0.005 * lightDistance;

                vec3 s = normalize(cross(-lightDirection, vec3(0.0, 1.0, 0.0)));
                vec3 u = cross(s, -lightDirection);
                mat3 sampleRotation = mat3(s, u, lightDirection);
                float shadowBias = 0.2 * (1.0 - dot(_fragmentSurface.NormalWorld, lightDirection)) + 0.01;
                float occluderCount = 0.0;
            
                float sampleAngle = shadowSampleSeed * PI;
                float t = sin(sampleAngle);
	            float d = cos(sampleAngle);
	            mat2 vogelRotation = mat2(d, -t, t, d);
            
                for (int i = 0; i < SHADOWSAMPLECOUNT; i++)
                {
                    vec3 samplePoint = sampleRotation * vec3(vogelRotation * VOGELDISKPOINTS[i] * VOGELPOINTRATIO * penumbra, 1.0);
                    float sampleDepth = SamplePointShadowDepth(samplePoint, shadowAtlasStart, shadowAtlasSize, shadowClipping);
                    occluderCount += lightDistance > sampleDepth + shadowBias ? 1 : 0;
                }
  
                surfaceColor *= 1 - (occluderCount / float(SHADOWSAMPLECOUNT));
            }

            OutputColor.xyz += surfaceColor + _pointLights[i].Color.w * _fragmentMaterial.Albedo * lightColor * attenuation;
        }
    }   
    
    for(int i = 0; i < _spotLights.length(); i++)
    {
        vec3 lightDiff = _spotLights[i].Position.xyz - _fragmentSurface.Position.xyz;
        vec3 lightColor = _spotLights[i].Color.xyz;
        float lightDistance = length(lightDiff);

        if (_spotLights[i].Range.x > lightDistance)
        {
            vec3 lightDirection = normalize(lightDiff);
            vec3 halfwayDirection = normalize(lightDirection + -_fragmentSurface.ViewDirection);
            float attenuation = pow(1 - clamp(lightDistance / _spotLights[i].Range.x, 0, 1), 3.0);

            float outerAngle = _spotLights[i].Position.w;
            float innerAngle = _spotLights[i].Direction.w;
            float theta = dot(lightDirection, normalize(_spotLights[i].Direction.xyz));
            float epsilon = innerAngle - outerAngle;
            float spotIntensity = clamp((theta - outerAngle) / epsilon, 0.0, 1.0);
            vec3 specularColor = mix(lightColor, _fragmentMaterial.Albedo * lightColor, _fragmentMaterial.MRO.x);
            vec3 surfaceColor = blinn_phong(_fragmentMaterial.Albedo, specularColor, _fragmentMaterial.MRO.y, _fragmentMaterial.Normal, halfwayDirection, lightDirection, lightColor) * spotIntensity * attenuation;

            if (_spotShadows[i].Strength.x > 0.001)
            {
                float shadowWidth = _spotShadows[i].Strength.w;
                vec2 shadowClipping = _spotShadows[i].Strength.yz;
                vec2 shadowAtlasStart = _spotShadows[i].Area.xy;
                vec2 shadowAtlasSize = _spotShadows[i].Area.zw;

                vec4 shadowWorldPosition = _spotShadows[i].Space * _fragmentSurface.Position;
                vec3 shadowScreenPosition = (shadowWorldPosition.xyz / shadowWorldPosition.w) * 0.5 + 0.5;
                float shadowBias = 0.0001 * (1.0 - dot(_fragmentSurface.NormalWorld, lightDirection)) + 0.000;
                float penumbra = 0.001 * lightDistance;
                float occluderCount = 0.0;
            
                float sampleAngle = shadowSampleSeed * PI;
                float s = sin(sampleAngle);
	            float c = cos(sampleAngle);
	            mat2 sampleRotation = mat2(c, -s, s, c);

                for (int i = 0; i < SHADOWSAMPLECOUNT; i++)
                {
                    vec2 vogelPoint = sampleRotation * VOGELDISKPOINTS[i] * VOGELPOINTRATIO * penumbra;
                    vec2 samplePoint = shadowAtlasStart + (shadowScreenPosition.xy + vogelPoint) * shadowAtlasSize;
                    float sampleDepth = texture(ShadowMap, samplePoint).r;

                    occluderCount += shadowScreenPosition.z > (sampleDepth + shadowBias) ? 1 : 0;
                }
                surfaceColor *= 1 - (occluderCount / float(SHADOWSAMPLECOUNT));
            }

            OutputColor.xyz += surfaceColor + _spotLights[i].Color.w * _fragmentMaterial.Albedo * lightColor * attenuation;
        }
    }
}