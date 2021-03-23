#version 430 core
in VertexUV
{
    vec3 UV0;
} _vertexUV;

uniform samplerCube ReflectionMap;
out vec4 FragColor;

void main()
{    
    FragColor = texture(ReflectionMap, _vertexUV.UV0);
}