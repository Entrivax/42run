#version 400

in vec2 texCoord;
out vec4 outColor;

uniform sampler2D tex;
uniform vec2 sprite;

void main()
{
    outColor = texture(tex, vec2(texCoord) + sprite);
}