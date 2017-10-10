#version 400

in vec2 ppos;
in vec2 uv;

uniform vec3 col;

out vec4 color;

vec3 hsv2rgb(vec3 c) {
	vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
	vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
	return c.z * mix(K.xxx, clamp(p-K.xxx, 0.0, 1.0), c.y);
}

void main(void)
{
	color = vec4(col, 1);//vec4(hsv2rgb(vec3(ppos.x, 1.0, 1.0)), 1.0);
}
