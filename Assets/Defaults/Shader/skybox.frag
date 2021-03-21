#version 430 core
in VertexOut
{
    vec3 UV0;
} _vertex;

uniform samplerCube ReflectionMap;
out vec4 FragColor;

void main()
{    
    FragColor = texture(ReflectionMap, _vertex.UV0);
}