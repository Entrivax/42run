#version 400

in vec3 _pos;
in vec2 _uv;

out vec3 texCoord;

uniform float texNum;
uniform mat4 proj;
uniform mat4 view;

void main(void)
{
	gl_Position = proj * view * vec4(_pos, 1.0);
	texCoord = vec3(_uv, texNum);
}