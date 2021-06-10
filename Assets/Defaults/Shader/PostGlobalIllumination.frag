#version 430 core

in VertexScreenQuad
{
    vec4 Position;
    vec2 UV0;
}
_vertexScreenQuad;


vec3 random_hemisphere(vec3 p, vec3 normal)
{
    p = fract(p);
    p *= 5461.5461;
	p = fract(p * vec3(.1031, .1030, .0973));
    p += dot(p, p.yxz+33.33);
    
    vec3 direction = normalize(fract((p.xxy + p.yxx)*p.zyx) * 2.0 - 1.0);
    return direction * sign(dot(direction, -normal));
}


uniform sampler2D BufferMap;
uniform sampler2D DeferredMRO;
uniform sampler2D DeferredPosition;
uniform sampler2D DeferredNormalTexture;

uniform mat4 Projection;
uniform vec4 ViewPosition;

out vec4 OutputColor;


void main()
{
    vec2 texelSize = 1.0 / textureSize(DeferredPosition, 0);
    vec3 position = texture(DeferredPosition, _vertexScreenQuad.UV0).xyz;
    vec3 normal = texture(DeferredNormalTexture, _vertexScreenQuad.UV0).xyz;

    vec3 inderectColor = vec3(0.0);
    vec2 sampleSSPosition = vec2(0.0);
    vec2 sampleDirection = vec2(0.0);
    vec4 worldPosition = vec4(0.0);
    vec4 worldDirection = vec4(0.0);

    int samples = 1;
    for(int i = 0; i < samples; i++)
    {
        vec3 seed = position + vec3(_vertexScreenQuad.UV0, 0.0) + worldDirection.xyz;

        worldDirection = vec4(random_hemisphere(seed, normal), 0.0);
        sampleDirection = (Projection * worldDirection).xy;

        if (abs(sampleDirection.x) > abs(sampleDirection.y))
            sampleDirection = vec2(sign(sampleDirection.x), sampleDirection.y / sampleDirection.x);
        else
            sampleDirection = vec2(sampleDirection.x / sampleDirection.y, sign(sampleDirection.y));
            
        sampleDirection *= texelSize;
        sampleSSPosition = _vertexScreenQuad.UV0;
        worldPosition = vec4(position, 1.0);

        vec3 testPosition = worldPosition.xyz;
        
        while (true)
        {
            sampleSSPosition += sampleDirection * 10.0;

            vec4 newPosition = texture(DeferredPosition, sampleSSPosition);
            minDepth += testPosition * (1.0 / length(newPosition - worldPosition));
                        
            worldPosition = newPosition;

            OutputColor = sampleWSPosition;
            return;

        }

    }

    OutputColor = vec4(samples / inderectColor, 1.0);
}