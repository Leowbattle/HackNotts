#version 330 core

uniform vec3 u_camPos;

in vec3 pos;

out vec4 fragColour;

// https://gist.github.com/wwwtyro/beecc31d65d1004f5a9d
float raySphereIntersect(vec3 r0, vec3 rd, vec3 s0, float sr) {
    // - r0: ray origin
    // - rd: normalized ray direction
    // - s0: sphere center
    // - sr: sphere radius
    // - Returns distance from r0 to first intersecion with sphere,
    //   or -1.0 if no intersection.
    float a = dot(rd, rd);
    vec3 s0_r0 = r0 - s0;
    float b = 2.0 * dot(rd, s0_r0);
    float c = dot(s0_r0, s0_r0) - (sr * sr);
    if (b*b - 4.0*a*c < 0.0) {
        return -1.0;
    }
    return (-b - sqrt((b*b) - 4.0*a*c))/(2.0*a);
}

void main() {
    vec3 dir = normalize(pos - u_camPos);
    float t = raySphereIntersect(u_camPos, dir, vec3(0.), 1);

    if (t == -1) {
        fragColour = vec4(1.);
    }
    else {
        vec3 p = u_camPos + t * dir;
        vec3 normal = p;

        fragColour = vec4(p, 1.);
    }
}