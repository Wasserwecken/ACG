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

const int SHADOWSAMPLECOUNT = 16;
const float SHADOWSAMPLECOUNTSQRT = sqrt(SHADOWSAMPLECOUNT);
const float VOGELPOINTRATIO = 32 / SHADOWSAMPLECOUNT;

// ENVIRONMENT
uniform samplerCube SkyboxMap;
uniform sampler2D ReflectionMap;
uniform sampler2D ShadowMap;

// FRAGMENT STRUCTS
struct FragmentMaterial
{
    vec3 Albedo;
    vec3 MRO;
    vec3 Emmision;
    vec3 Normal;
    vec3 FresnelColor;
    float NdV;
} _fragmentMaterial;

struct FragmentSurface
{
    vec4 Position;
    vec3 ViewDiff;
    vec3 ViewDirection;
    vec3 NormalWorld;
    float ViewLength;
} _fragmentSurface;

struct LightInfo
{
    vec3 Color;
    vec3 Direction;
    float Ambient;
} _lightInfo;

// INPUT SPECIFIC UNIFORMS
uniform sampler2D DeferredPosition;
uniform sampler2D DeferredAlbedo;
uniform sampler2D DeferredNormalSurface;
uniform sampler2D DeferredNormalTexture;
uniform sampler2D DeferredMRO;
uniform sampler2D DeferredEmission;

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

vec3 blinn_phong(vec3 surfaceDiffuse, vec3 surfaceSpecular, float roughness, float occlusion, vec3 normal, vec3 halfway, vec3 lightDirection, vec3 lightColor)
{
    roughness = 1.0 - roughness;
    roughness = roughness * roughness;

    float glossy = mix(128.0, 1.0, roughness);
    float luminance = max(dot(normal, lightDirection), 0.0);
    vec3 specular = roughness * pow(max(dot(normal, halfway), 0.0), glossy) * surfaceSpecular;

    return (surfaceDiffuse * occlusion + specular) * lightColor * luminance;
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


void CubeAtlasUV(vec3 direction, vec2 atlasStart, vec2 atlasSize, out vec2 sampleUV, out vec2 faceUV)
{
	vec3 directionAbs = abs(direction);
	float ma;
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
    sampleUV = atlasStart + ((faceUV + faceId) / vec2(3.0, 2.0)) * atlasSize;
}

vec3 FresnelSchlick(float nDotV, vec3 f0, float roughness)
{
    return f0 + (1.0 - f0) * pow(1.0 - nDotV, 5.0);
}

float DistributionTrowbridgeReitzGGX(float nDotH, float roughness)
{
    roughness *= roughness;
    roughness *= roughness;
    nDotH *= nDotH;

    float denom = (nDotH * (roughness - 1.0) + 1.0);
    return roughness / (PI * denom * denom);
}

float GeometrySchlickGGX(float nDotV, float roughness)
{
    float r = (roughness + 1.0);
    float k = (r*r) / 8.0;
    float denom = nDotV * (1.0 - k) + k;
	
    return nDotV / denom;
}

float GeometrySmith(float nDotV, float nDotL, float roughness)
{
    float ggxView  = GeometrySchlickGGX(nDotV, roughness);
    float ggxLight  = GeometrySchlickGGX(nDotL, roughness);
	
    return ggxView * ggxLight;
}

vec3 PBRDRDF(FragmentMaterial material, FragmentSurface surface, LightInfo light)
{            
    vec3 halfway = normalize(light.Direction + surface.ViewDirection);
    float hDv = max(0.0, dot(halfway, surface.ViewDirection));
    float nDl = max(0.0, dot(material.Normal, light.Direction));
    float nDh = max(0.0, dot(material.Normal, halfway));
    vec3 fresnelColor = FresnelSchlick(hDv, material.FresnelColor, material.MRO.y);

    // SPECULAR
    float normalDistribution = DistributionTrowbridgeReitzGGX(nDh, material.MRO.y);
    float selfShadowing = GeometrySmith(material.NdV, nDl, material.MRO.y);

    vec3 numerator = fresnelColor * normalDistribution * selfShadowing;
    float denominator = 4 * material.NdV * nDl;
    vec3 specular = numerator / max(denominator, 0.001);

    // DIFFUSE
    vec3 diffuse = vec3(1.0 - fresnelColor) * (1.0 - material.MRO.x) * material.Albedo;

    // MIX
    return ((diffuse / PI) + specular) * nDl * light.Color * material.MRO.z;
}

vec3 PBRAmbient(FragmentMaterial material, LightInfo light)
{
    vec3 result = mix(vec3(1.0), material.Albedo, material.MRO.x);
    result *= material.MRO.y;
    result *= light.Color;
    result *= light.Ambient;
    return result;
}

vec3 Reflection(FragmentMaterial material, vec3 reflectionColor)
{
    vec3 result = mix(reflectionColor, material.Albedo * reflectionColor, material.MRO.x);
    result *= (1.0 - material.MRO.y);
    result *= (1.0 - material.MRO.y);
    result *= (1.0 - material.MRO.y);
    result *= (1.0 - material.MRO.y);
    result *= material.MRO.x;
    return result;
}

// SHADER OUTPUT
out vec4 OutputColor;

// MAIN
void main()
{    
    vec2 screenUV = (gl_FragCoord.xy / _viewDeferred.Resolution);
   
    _fragmentSurface.Position = texture(DeferredPosition, screenUV);
    _fragmentSurface.ViewDiff = _viewDeferred.ViewPosition.xyz - _fragmentSurface.Position.xyz;
    _fragmentSurface.ViewDirection = normalize(_fragmentSurface.ViewDiff);
    _fragmentSurface.NormalWorld = texture(DeferredNormalSurface, screenUV).xyz;
    _fragmentSurface.ViewLength = length(_fragmentSurface.ViewDiff);

    _fragmentMaterial.Albedo = pow(texture(DeferredAlbedo, screenUV).xyz, vec3(2.2));
    _fragmentMaterial.MRO = texture(DeferredMRO, screenUV).xyz;
    _fragmentMaterial.Emmision = texture(DeferredEmission, screenUV).xyz;
    _fragmentMaterial.Normal = texture(DeferredNormalTexture, screenUV).xyz;
    _fragmentMaterial.FresnelColor = mix(vec3(0.04), _fragmentMaterial.Albedo, _fragmentMaterial.MRO.x);
    _fragmentMaterial.NdV = max(0.0, dot(_fragmentMaterial.Normal, _fragmentSurface.ViewDirection));
            
            
    OutputColor = vec4(_fragmentMaterial.Emmision, 1.0);


    vec3 reflectionColor = vec3(0.0);
    vec3 reflectionDirection = reflect(-_fragmentSurface.ViewDirection, _fragmentMaterial.Normal);
    if (_reflectionProbes.length() > 0)
    {
        int probeId = -1;
        float closestDistance = 100.0;
        for (int i = 0; i < _reflectionProbes.length(); i++)
        {
            vec3 probeDiff = _reflectionProbes[i].Position.xyz - _fragmentSurface.Position.xyz;
            float probeDistance = dot(probeDiff, probeDiff);// * dot(probeDiff, _fragmentMaterial.Normal);

            if (probeDistance < closestDistance)
            {
                probeId = i;
                closestDistance = probeDistance;
            }
        }
            
        vec2 probeSampleUV, probeFaceUV;
        CubeAtlasUV(-reflectionDirection, _reflectionProbes[probeId].Area.xy, _reflectionProbes[probeId].Area.zw, probeSampleUV, probeFaceUV);
        reflectionColor = texture(ReflectionMap, probeSampleUV).xyz;
    }
    else
    {
        reflectionColor = texture(SkyboxMap, reflectionDirection).xyz;
    }

    OutputColor.xyz += Reflection(_fragmentMaterial, reflectionColor);

        
    float shadowSampleSeed = ShadowHash(gl_FragCoord.xy);
    vec2 ShadowMapPixels = vec2(textureSize(ShadowMap, 0));
    
    for(int i = 0; i < _directionalLights.length(); i++)
    {
        _lightInfo.Color = _directionalLights[i].Color.xyz;
        _lightInfo.Direction = _directionalLights[i].Direction.xyz;
        _lightInfo.Ambient = _directionalLights[i].Color.w;

        OutputColor.xyz += PBRAmbient(_fragmentMaterial, _lightInfo);
        vec3 surfaceColor = PBRDRDF(_fragmentMaterial, _fragmentSurface, _lightInfo);
        
        if (_directionalShadows[i].Strength.x > 0.001)
        {
            float shadowWidth = _directionalShadows[i].Strength.w;
            vec2 shadowClipping = _directionalShadows[i].Strength.yz;
            vec2 shadowAtlasStart = _directionalShadows[i].Area.xy;
            vec2 shadowAtlasSize = _directionalShadows[i].Area.zw;

            vec4 shadowWorldPosition = _directionalShadows[i].Space * _fragmentSurface.Position;
            vec3 shadowScreenPosition = (shadowWorldPosition.xyz / shadowWorldPosition.w) * 0.5 + 0.5;
            float shadowBias = 0.003 * (1.0 - dot(_fragmentSurface.NormalWorld, _lightInfo.Direction)) + 0.001;
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

        OutputColor.xyz += surfaceColor;
    }
    
    for(int i = 0; i < _pointLights.length(); i++)
    {
        vec3 lightDiff = _pointLights[i].Position.xyz - _fragmentSurface.Position.xyz;
        float lightDistance = length(lightDiff);
        
        if (_pointLights[i].Position.w > lightDistance)
        {
            _lightInfo.Color = _pointLights[i].Color.xyz;
            _lightInfo.Direction = normalize(lightDiff);
            _lightInfo.Ambient = _pointLights[i].Color.w;

            float attenuation = 1 - clamp(lightDistance / _pointLights[i].Position.w, 0, 1);
            attenuation *= attenuation * attenuation;

            OutputColor.xyz += PBRAmbient(_fragmentMaterial, _lightInfo) * attenuation;
            vec3 surfaceColor = PBRDRDF(_fragmentMaterial, _fragmentSurface, _lightInfo) * attenuation;

            if (_pointShadows[i].Strength.x > 0.001)
            {
                vec2 shadowClipping = vec2(_pointShadows[i].Strength.y, _pointLights[i].Position.w);
                vec2 shadowAtlasStart = _pointShadows[i].Area.xy;
                vec2 shadowAtlasSize = _pointShadows[i].Area.wz;
                float penumbra = 0.005 * lightDistance;

                vec3 s = normalize(cross(-_lightInfo.Direction, vec3(0.0, 1.0, 0.0)));
                vec3 u = cross(s, -_lightInfo.Direction);
                mat3 sampleRotation = mat3(s, u, _lightInfo.Direction);
                float shadowBias = 0.2 * (1.0 - dot(_fragmentSurface.NormalWorld, _lightInfo.Direction)) + 0.01;
                float occluderCount = 0.0;
            
                float sampleAngle = shadowSampleSeed * PI;
                float t = sin(sampleAngle);
	            float d = cos(sampleAngle);
	            mat2 vogelRotation = mat2(d, -t, t, d);
            
                for (int i = 0; i < SHADOWSAMPLECOUNT; i++)
                {
                    vec2 sampleUV, faceUV;
                    vec3 sampleDirection = sampleRotation * vec3(vogelRotation * VOGELDISKPOINTS[i] * VOGELPOINTRATIO * penumbra, 1.0);
                    CubeAtlasUV(sampleDirection, shadowAtlasStart, shadowAtlasSize, sampleUV, faceUV);

                    float depthCorrection = length(vec3(faceUV * 2.0 - 1.0, 1.0));
                    float sampleDepth = LinearizeDepth(texture(ShadowMap, sampleUV).r, shadowClipping.x, shadowClipping.y) * depthCorrection;

                    occluderCount += lightDistance > sampleDepth + shadowBias ? 1 : 0;
                }
  
                surfaceColor *= 1 - (occluderCount / float(SHADOWSAMPLECOUNT));
            }
            
            OutputColor.xyz += surfaceColor;
        }
    }   
    
    for(int i = 0; i < _spotLights.length(); i++)
    {
        vec3 lightDiff = _spotLights[i].Position.xyz - _fragmentSurface.Position.xyz;
        float lightDistance = length(lightDiff);

        if (_spotLights[i].Range.x > lightDistance)
        {
            _lightInfo.Color = _spotLights[i].Color.xyz;
            _lightInfo.Direction = normalize(lightDiff);
            _lightInfo.Ambient = _spotLights[i].Color.w;
            
            float outerAngle = _spotLights[i].Position.w;
            float innerAngle = _spotLights[i].Direction.w;
            float epsilon = innerAngle - outerAngle;
            float theta = dot(_lightInfo.Direction, normalize(_spotLights[i].Direction.xyz));
            float attenuation = 1 - clamp(lightDistance / _spotLights[i].Range.x, 0, 1);
            attenuation *= attenuation * attenuation;
            attenuation *= clamp((theta - outerAngle) / epsilon, 0.0, 1.0);

            OutputColor.xyz += PBRAmbient(_fragmentMaterial, _lightInfo) * attenuation;
            vec3 surfaceColor = PBRDRDF(_fragmentMaterial, _fragmentSurface, _lightInfo) * attenuation;

            if (_spotShadows[i].Strength.x > 0.001)
            {
                float shadowWidth = _spotShadows[i].Strength.w;
                vec2 shadowClipping = _spotShadows[i].Strength.yz;
                vec2 shadowAtlasStart = _spotShadows[i].Area.xy;
                vec2 shadowAtlasSize = _spotShadows[i].Area.zw;

                vec4 shadowWorldPosition = _spotShadows[i].Space * _fragmentSurface.Position;
                vec3 shadowScreenPosition = (shadowWorldPosition.xyz / shadowWorldPosition.w) * 0.5 + 0.5;
                float shadowBias = 0.0001 * (1.0 - dot(_fragmentSurface.NormalWorld, _lightInfo.Direction)) + 0.000;
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
            
            OutputColor.xyz += surfaceColor;
        }
    }
}