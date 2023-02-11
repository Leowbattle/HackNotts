#version 330 core

layout(location = 0) in vec3 a_pos;

uniform mat4 u_mvps[10];
uniform mat4 u_models[10];

out vec3 pos;

void main() {
	mat4 mvp = u_mvps[gl_InstanceID];
	mat4 u_models = u_models[gl_InstanceID];

	gl_Position = mvp * vec4(a_pos, 1.);

	pos = (u_model * vec4(a_pos, 1.)).xyz;
}
