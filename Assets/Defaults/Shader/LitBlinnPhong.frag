#version 430 core
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

struct PointLight
{
    vec4 Color;
    vec4 Position;
    vec4 ShadowArea;
    vec4 ShadowStrength;
};

struct SpotLight
{
 vec4 Color;
 vec4 Position;
 vec4 Direction;
};

layout (std430) buffer ShaderDirectionalLight {
    DirectionalLight _directionalLights[];
};

layout (std430) buffer ShaderPointLight {
    PointLight _pointLights[];
};

layout (std430) buffer ShaderSpotLight {
    SpotLight _spotLights[];
};

// ENVIRONMENT
uniform samplerCube ReflectionMap;
uniform sampler2D DirectionalShadowMap;
uniform sampler2D PointShadowMap;

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
vec3 blinn_phong(vec3 surfaceDiffuse, vec3 surfaceSpecular, float glossy, vec3 normal, vec3 halfway, vec3 lightDirection, vec3 lightColor)
{
    float luminance = max(dot(normal, lightDirection), 0.0);
    vec3 specular = surfaceSpecular * pow(max(dot(normal, halfway), 0.0), glossy);

    return (surfaceDiffuse + specular) * lightColor * luminance;
}

vec2 sample_Cube(vec3 direction, out float depthScale)
{
	vec3 directionAbs = abs(direction);
	float ma;
	vec2 uv;
    vec2 faceId;
    
	if(directionAbs.z >= directionAbs.x && directionAbs.z >= directionAbs.y)
	{
        faceId.x = 0.0;
		faceId.y = direction.z < 0.0 ? 0.0 : 1.0;
		ma = 0.5 / directionAbs.z;
		uv = vec2(direction.z > 0.0 ? -direction.x : direction.x, -direction.y);
	}
	else if(directionAbs.y >= directionAbs.x)
	{
        faceId.x = 1.0;
		faceId.y = direction.y < 0.0 ? 0.0 : 1.0;
		ma = 0.5 / directionAbs.y;
		uv = vec2(direction.z, direction.y < 0.0 ? -direction.x : direction.x);
	}
	else
	{
        faceId.x = 2.0;
		faceId.y = direction.x < 0.0 ? 0.0 : 1.0;
		ma = 0.5 / directionAbs.x;
		uv = vec2(direction.x > 0.0 ? direction.z : -direction.z, -direction.y);
	}

    uv = uv * ma + 0.5;
    depthScale = length(vec3(uv * 2.0 - 1.0, 1.0));

	return (uv + faceId) / vec2(3.0, 2.0);
}

float evaluate_shadow(vec4 shadowPosition, vec4 shadowArea, vec3 surfaceNormal, vec3 lightDirection)
{
    vec3 projectedPosition = (shadowPosition.xyz / shadowPosition.w) * 0.5 + 0.5;
    vec2 shadowUV = shadowArea.xy + projectedPosition.xy * shadowArea.zw;
    float bias = max(0.05 * (1.0 - dot(surfaceNormal, lightDirection)), 0.001);
    float shadowDepth = texture(DirectionalShadowMap, shadowUV).r + bias;

    return projectedPosition.z < shadowDepth ? 1.0 : 0.0;
}

vec3 evaluate_lights(vec3 baseColor, float metalic, float roughness, vec3 surfaceNormal)
{
    vec3 viewDirection = normalize(_viewSpace.ViewPosition - _vertexPosition.PositionWorld.xyz);
    vec3 reflectionColor = texture(ReflectionMap, reflect(-viewDirection, surfaceNormal)).xyz;
    vec3 specularColor = mix(vec3(1.0), baseColor, metalic);
    float glossy = mix(128.0, 0.0, roughness * roughness);
    vec3 result = reflectionColor * baseColor * metalic;
    
    for(int i = 0; i < _directionalLights.length(); i++)
    {
        vec3 lightColor = _directionalLights[i].Color.xyz;
        vec3 lightDirection = _directionalLights[i].Direction.xyz;
        vec3 halfwayDirection = normalize(lightDirection + viewDirection);

        vec3 surfaceColor = blinn_phong(baseColor, specularColor, glossy, surfaceNormal, halfwayDirection, lightDirection, lightColor);

        if (_directionalLights[i].ShadowStrength.x > 0.001)
        {
            vec4 shadowSpacePosition = _directionalLights[i].ShadowSpace * _vertexPosition.PositionWorld;
            surfaceColor *= evaluate_shadow(shadowSpacePosition, _directionalLights[i].ShadowArea, surfaceNormal, lightDirection);
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
            float depthScale;
            vec2 shadowUV = _pointLights[i].ShadowArea.xy + sample_Cube(lightDirection, depthScale) * _pointLights[i].ShadowArea.zw;
            float lightDepth = (lightDistance / depthScale);
            float near = _pointLights[i].ShadowStrength.y;
            float far = _pointLights[i].Position.w;

            float shadowDepth = texture(PointShadowMap, shadowUV).r * 2.0 - 1.0;
            shadowDepth = (2.0 * near * far) / (far + near - shadowDepth * (far - near));
            shadowDepth += max(0.05 * (1.0 - dot(surfaceNormal, lightDirection)), 0.1);

            surfaceColor *= vec3(lightDepth < shadowDepth ? 1.0 : 0.0);
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
    vec4 baseColor = texture(BaseColorMap, _vertexUV.UV0);
    vec2 metallicRoughness = texture(MetallicRoughnessMap, _vertexUV.UV0).yz * MREO.xy;
    vec3 emmision = texture(EmissiveMap, _vertexUV.UV0).xyz * MREO.z;
    float occlusion = texture(OcclusionMap, _vertexUV.UV0).x * MREO.w;
    vec3 textureNormal = normalize(_vertexTangent.TangentSpaceWorld * (texture(NormalMap, _vertexUV.UV0).xyz * 2.0 - 1.0));

    vec3 surfaceNormal = mix(_vertexNormal.NormalWorld, textureNormal, Normal);
    vec3 surfaceColor = emmision + evaluate_lights(baseColor.xyz, metallicRoughness.y, metallicRoughness.x, surfaceNormal);
    vec3 corrected = pow(surfaceColor, vec3(0.454545454545));

    OutputColor = vec4(corrected, baseColor.w);
}