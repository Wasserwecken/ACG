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

vec3 evaluate_lights(vec3 baseColor, float metalic, float roughness, vec3 surfaceNormal, vec3 positionViewDiff)
{
    vec3 viewDirection = -normalize(positionViewDiff);
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
            vec4 shadowPosition = _directionalLights[i].ShadowSpace * _vertexPosition.PositionWorld;

            vec3 projectedPosition = (shadowPosition.xyz / shadowPosition.w) * 0.5 + 0.5;
            vec2 shadowUV = _directionalLights[i].ShadowArea.xy + projectedPosition.xy * _directionalLights[i].ShadowArea.zw;
            float bias = 0.002 * (1.0 - max(dot(normalize(_vertexNormal.NormalWorld), lightDirection), 0.0) + 0.001);
            float shadowDepth = texture(DirectionalShadowMap, shadowUV).r;

            surfaceColor *= projectedPosition.z < shadowDepth + bias ? 1.0 : 0.0;
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
            float near = _pointLights[i].ShadowStrength.y;
            float far = _pointLights[i].Position.w;
            float depthScale;
            vec2 shadowUV = _pointLights[i].ShadowArea.xy + sample_Cube(lightDirection, depthScale) * _pointLights[i].ShadowArea.wz;
            float lightDepth = ((1 / (lightDistance / depthScale)) - (1 / near)) / ((1 / far) - (1 / near));
            float shadowDepth = texture(PointShadowMap, shadowUV).r;
            float bias = 0.002 * (1.0 - max(dot(normalize(_vertexNormal.NormalWorld), lightDirection), 0.0) + 0.001);

            surfaceColor *= lightDepth < shadowDepth + bias ? 1.0 : 0.0;
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