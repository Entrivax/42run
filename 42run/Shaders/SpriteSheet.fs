#version 400

in vec3 texCoord;
out vec4 outColor;

uniform sampler2DArray tex;

void main()
{
    outColor = texture(tex, vec3(texCoord));
}