#version 330

out vec4 outputColor;
in vec2 texCoord;

uniform sampler2D texture1;
uniform float time;
uniform sampler2D texture2;

void main()
{
    outputColor = mix(texture(texture1, texCoord + vec2(time * 0.1)), texture(texture2, texCoord), 0.2);
}