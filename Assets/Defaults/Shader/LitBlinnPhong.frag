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


// INPUT VERTEX
in VertexPosition
{
    vec4 PositionLocal;
    vec4 PositionView;
    vec4 PositionWorld;
} _vertexPosition;

in VertexNormal
{
    vec3 NormalLocal;
    vec3 NormalWorld;
    vec3 NormalView;
} _vertexNormal;

in VertexTangent
{
    mat3 TangentSpaceLocal;
    mat3 TangentSpaceWorld;
    mat3 TangentSpaceView;
} _vertexTangent;

in VertexUV
{
    vec2 UV0;
    vec2 UV1;
} _vertexUV;

in VertexColor
{
    vec4 Color;
} _vertexColor;

// INPUT GLOBAL UNIFORMS
layout (std430) buffer ShaderTime {
    float Frame;
    float Fixed;
    float Total;
    float TotalSin;
} _time;

layout (std430) buffer ShaderPrimitiveSpace {
    mat4 LocalToWorld;
    mat4 LocalToView;
    mat4 LocalToProjection;
    mat4 LocalToWorldRotation;
    mat4 LocalToViewRotation;
    mat4 LocalToProjectionRotation;
} _primitiveSpace;

layout (std430) buffer ShaderViewSpace {
    mat4 WorldToView;
    mat4 WorldToProjection;
    mat4 WorldToViewRotation;
    mat4 WorldToProjectionRotation;
    vec3 ViewPosition;
    vec3 ViewDirection;
    mat4 ViewProjection;
} _viewSpace;

layout (std430) buffer ShaderShadowSpace {
    mat4 ShadowSpace;
} _shadowSpace;

// INPUT GLOBAL UNIFORMS LIGHT
struct DirectionalLight
{
    vec4 Color;
    vec4 Direction;
    vec4 ShadowArea;
    mat4 ShadowSpace;
    vec4 ShadowStrength;
};

layout (std430) buffer ShaderDirectionalLight {
    DirectionalLight _directionalLights[];
};

struct PointLight
{
    vec4 Color;
    vec4 Position;
    vec4 ShadowArea;
    vec4 ShadowStrength;
};

layout (std430) buffer ShaderPointLight {
    PointLight _pointLights[];
};

struct SpotLight
{
 vec4 Color;
 vec4 Position;
 vec4 Direction;
};

layout (std430) buffer ShaderSpotLight {
    SpotLight _spotLights[];
};

// ENVIRONMENT
uniform samplerCube ReflectionMap;
uniform sampler2D DirectionalShadowMap;
uniform sampler2D PointShadowMap;

// INPUT GENERAL UNIFORMS
uniform float AlphaCutoff;

// INPUT SPECIFIC UNIFORMS
uniform vec4 BaseColor;
uniform vec4 MREO;
uniform float Normal;
uniform sampler2D BaseColorMap;
uniform sampler2D MetallicRoughnessMap;
uniform sampler2D EmissiveMap;
uniform sampler2D OcclusionMap;
uniform sampler2D NormalMap;

// SHADER OUTPUT
out vec4 OutputColor;

// LOGIC
mat3 cotangent_frame(vec3 N, vec3 p, vec2 uv)
{
    // http://www.thetenthplanet.de/archives/1180
    // get edge vectors of the pixel triangle
    vec3 dp1 = dFdx( p );
    vec3 dp2 = dFdy( p );
    vec2 duv1 = dFdx( uv );
    vec2 duv2 = dFdy( uv );
 
    // solve the linear system
    vec3 dp2perp = cross( dp2, N );
    vec3 dp1perp = cross( N, dp1 );
    vec3 T = dp2perp * duv1.x + dp1perp * duv2.x;
    vec3 B = dp2perp * duv1.y + dp1perp * duv2.y;
 
    // construct a scale-invariant frame 
    float invmax = inversesqrt( max( dot(T,T), dot(B,B) ) );
    return mat3( T * invmax, B * invmax, N );
}

vec2 VogelDiskSample(int sampleIndex, int samplesCount, float phi)
{
  float goldenAngle = 2.399963229;
  float theta = sampleIndex * goldenAngle + phi;
  float r = sqrt(sampleIndex + 0.5) / sqrt(samplesCount);
  return vec2(r * cos(theta), r * sin(theta));
}

vec3 VogelConeSample(int sampleIndex, int samplesCount, float phi, vec3 direction, float angle)
{
    float goldenAngle = 2.399963229;
    float theta = sampleIndex * goldenAngle + phi;
    float z = 1.0 - ((1.0 - cos(angle)) * sqrt(sampleIndex + 0.5) / sqrt(samplesCount));
    float r = sqrt(1 - (z * z));
    vec3 result = vec3(r * cos(theta), r * sin(theta), z);
    
    // rotate towards view direction
    vec3 f = -direction;
    vec3 s = normalize(cross(f, vec3(0.0, 1.0, 0.0)));
    vec3 u = cross(s, f);
    mat3 rotationMatrix = mat3(s, u, -f);

    return rotationMatrix * result;
}

float ShadowHash(vec2 point)
{
  vec3 seed = vec3(0.06711056f, 0.00583715f, 52.9829189f);
  return fract(seed.z * fract(dot(point, seed.xy)));
}

vec3 blinn_phong(vec3 surfaceDiffuse, vec3 surfaceSpecular, float glossy, vec3 normal, vec3 halfway, vec3 lightDirection, vec3 lightColor)
{
    float luminance = max(dot(normal, lightDirection), 0.0);
    vec3 specular = surfaceSpecular * pow(max(dot(normal, halfway), 0.0), glossy);

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
    return LinearizeDepth(texture(PointShadowMap, shadowUV).r, clipping.x, clipping.y) * depthCorrection;
}

vec3 evaluate_lights(vec3 baseColor, float metalic, float roughness, vec3 surfaceNormal, vec3 positionViewDiff)
{
    vec3 viewDirection = -normalize(positionViewDiff);
    vec3 reflectionColor = texture(ReflectionMap, reflect(-viewDirection, surfaceNormal)).xyz;
    vec3 specularColor = mix(vec3(1.0), baseColor, metalic);
    float glossy = mix(128.0, 0.0, roughness * roughness);
    vec3 result = reflectionColor * baseColor * metalic;
    
    float shadowSampleSeed = ShadowHash(gl_FragCoord.xy);
    int shadowSampleCount = 8;
    vec2 directionalShadowPixels = vec2(textureSize(DirectionalShadowMap, 0));
    vec2 pointShadowPixels = vec2(textureSize(PointShadowMap, 0));
    
    for(int i = 0; i < _directionalLights.length(); i++)
    {
        vec3 lightColor = _directionalLights[i].Color.xyz;
        vec3 lightDirection = _directionalLights[i].Direction.xyz;
        vec3 halfwayDirection = normalize(lightDirection + viewDirection);
        vec3 surfaceColor = blinn_phong(baseColor, specularColor, glossy, surfaceNormal, halfwayDirection, lightDirection, lightColor);

        if (_directionalLights[i].ShadowStrength.x > 0.001)
        {
            float shadowWidth = _directionalLights[i].ShadowStrength.w;
            vec2 shadowClipping = _directionalLights[i].ShadowStrength.yz;
            vec2 shadowAtlasStart = _directionalLights[i].ShadowArea.xy;
            vec2 shadowAtlasSize = _directionalLights[i].ShadowArea.zw;

            vec4 shadowWorldPosition = _directionalLights[i].ShadowSpace * _vertexPosition.PositionWorld;
            vec3 shadowScreenPosition = (shadowWorldPosition.xyz / shadowWorldPosition.w) * 0.5 + 0.5;
            float shadowBias = 0.003 * (1.0 - max(dot(normalize(_vertexNormal.NormalWorld), lightDirection), 0.0)) + 0.001;
            float penumbra = 0.1 * (shadowWidth / directionalShadowPixels.x);
            float occluderCount = 0.0;
            
            for (int i = 0; i < shadowSampleCount; i++)
            {
                vec2 sampleUV = shadowScreenPosition.xy + VogelDiskSample(i, shadowSampleCount, shadowSampleSeed * PI) * penumbra;
                float sampleDepth = texture(DirectionalShadowMap, shadowAtlasStart + sampleUV * shadowAtlasSize).r;

                occluderCount += shadowScreenPosition.z > sampleDepth + shadowBias ? 1 : 0;
            }

            surfaceColor *= 1 - (occluderCount / float(shadowSampleCount));
        }

        result += surfaceColor + _directionalLights[i].Color.w * baseColor * lightColor;
    }
    
    for(int i = 0; i < _pointLights.length(); i++)
    {
        vec3 lightDiff = _pointLights[i].Position.xyz - _vertexPosition.PositionWorld.xyz;
        vec3 lightColor = _pointLights[i].Color.xyz;
        float lightDistance = length(lightDiff);
        vec3 lightDirection = normalize(lightDiff);
        vec3 halfwayDirection = normalize(lightDirection + viewDirection);
        float attenuation = pow(1 - clamp(lightDistance / _pointLights[i].Position.w, 0, 1), 3.0);

        vec3 surfaceColor = blinn_phong(baseColor, specularColor, glossy, surfaceNormal, halfwayDirection, lightDirection, lightColor) * attenuation;

        if (_pointLights[i].ShadowStrength.x > 0.001 && _pointLights[i].Position.w > lightDistance)
        {
            vec2 shadowClipping = vec2(_pointLights[i].ShadowStrength.y, _pointLights[i].Position.w);
            vec2 shadowAtlasStart = _pointLights[i].ShadowArea.xy;
            vec2 shadowAtlasSize = _pointLights[i].ShadowArea.wz;

            float shadowBias = 0.2 * (1.0 - max(dot(normalize(_vertexNormal.NormalWorld), lightDirection), 0.0)) + 0.01;
            float penumbra = 0.01 * lightDistance;
            float occluderCount = 0.0;

            for (int i = 0; i < shadowSampleCount; i++)
            {
                vec3 sampleDirection = VogelConeSample(i, shadowSampleCount, shadowSampleSeed * PI, lightDirection, penumbra);
                float sampleDepth = SamplePointShadowDepth(sampleDirection, shadowAtlasStart, shadowAtlasSize, shadowClipping);

                occluderCount += lightDistance > sampleDepth + shadowBias ? 1 : 0;
            }
  
            surfaceColor *= 1 - (occluderCount / float(shadowSampleCount));
        }

        result += surfaceColor + _pointLights[i].Color.w * baseColor * lightColor * attenuation;
    }   
    
    for(int i = 0; i < _spotLights.length(); i++)
    {
        vec3 lightDiff = _spotLights[i].Position.xyz - _vertexPosition.PositionWorld.xyz;
        vec3 lightColor = _spotLights[i].Color.xyz;
        float lightDistance = length(lightDiff);
        vec3 lightDirection = normalize(lightDiff);
        vec3 halfwayDirection = normalize(lightDirection + _viewSpace.ViewDirection);
        float attenuationSqrared = 1.0 / (1.0 + (lightDistance * lightDistance));
        float attenuationLinear = 1.0 / (1.0 + lightDistance);

        float outerAngle = _spotLights[i].Position.w;
        float innerAngle = _spotLights[i].Direction.w;
        float theta = dot(lightDirection, normalize(_spotLights[i].Direction.xyz));
        float epsilon = innerAngle - outerAngle;
        float spotIntensity = clamp((theta - outerAngle) / epsilon, 0.0, 1.0);   

        result += blinn_phong(baseColor, specularColor, glossy, surfaceNormal, halfwayDirection, lightDirection, lightColor) * spotIntensity * attenuationSqrared;
        result += _spotLights[i].Color.w * baseColor * lightColor * attenuationLinear;
    }

    return result;
}

void main()
{
    vec4 baseColor = pow(texture(BaseColorMap, _vertexUV.UV0), vec4(2.2));

   if (baseColor.w < AlphaCutoff)
        discard;

    vec3 positionViewDiff = _vertexPosition.PositionWorld.xyz - _viewSpace.ViewPosition;
    vec2 metallicRoughness = texture(MetallicRoughnessMap, _vertexUV.UV0).yz * MREO.xy;
    vec3 emmision = texture(EmissiveMap, _vertexUV.UV0).xyz * MREO.z;
    float occlusion = texture(OcclusionMap, _vertexUV.UV0).x * MREO.w;

    vec3 textureNormal = (texture(NormalMap, _vertexUV.UV0).xyz * 2.0 - 1.0) * vec3(1.0, 1.0, 0.3 / Normal);
    mat3 tangenSpace = cotangent_frame(normalize(_vertexNormal.NormalLocal), positionViewDiff, _vertexUV.UV0);
    vec3 surfaceNormal = normalize(tangenSpace * textureNormal);

    vec3 surfaceColor = emmision + evaluate_lights(baseColor.xyz, metallicRoughness.y, metallicRoughness.x, surfaceNormal, positionViewDiff);
    vec3 corrected = pow(surfaceColor, vec3(0.454545454545));

    OutputColor = vec4(corrected, baseColor.w);
}