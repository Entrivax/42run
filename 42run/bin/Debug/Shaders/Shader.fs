#version 400

#define FOG_START 20
#define FOG_END 60

in vec2 ppos;
in vec2 uv;
in float dist;

uniform vec3 col;
uniform sampler2D tex;

out vec4 color;

vec3 hsv2rgb(vec3 c) {
	vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
	vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
	return c.z * mix(K.xxx, clamp(p-K.xxx, 0.0, 1.0), c.y);
}

void main(void)
{
	float fog = clamp((FOG_END - dist) / (FOG_END - FOG_START), 0.0, 1.0);
	color = mix(texture(tex, uv), vec4(0, 0, 0, 1), 1 - fog);//vec4(hsv2rgb(vec3(ppos.x, 1.0, 1.0)), 1.0);
}
