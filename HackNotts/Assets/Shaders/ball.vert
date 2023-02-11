#version 330 core

layout(location = 0) in vec3 a_pos;

uniform mat4 u_mvps[100];
uniform mat4 u_models[100];

uniform mat4 u_view;

out vec3 pos;
flat out int instanceID;

void main() {
	mat4 mvp = u_mvps[gl_InstanceID];
	mat4 model = u_models[gl_InstanceID];

	gl_Position = mvp * vec4(a_pos, 1.);

	pos = (model * vec4(a_pos, 1.)).xyz;

	instanceID = gl_InstanceID;
}
