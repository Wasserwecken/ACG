#version 430 core
in VertexOut
{
    vec3 UV0;
} _vertex;

uniform samplerCube SkyMap;
out vec4 FragColor;

void main()
{    
    FragColor = texture(SkyMap, _vertex.UV0);
}