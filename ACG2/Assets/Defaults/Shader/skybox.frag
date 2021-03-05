#version 430 core
in VertexOut
{
    vec3 UV0;
} _vertex;

uniform samplerCube skybox;
out vec4 FragColor;


void main()
{    
    FragColor = texture(skybox, _vertex.UV0);
}