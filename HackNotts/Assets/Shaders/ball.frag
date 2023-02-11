#version 330 core

uniform vec3 u_pos[100];
uniform mat4 u_mvps[100];
uniform mat4 u_models[100];

uniform mat4 u_view;
uniform mat4 u_proj;

uniform int u_num;

uniform vec3 u_camPos;

uniform samplerCube u_sky;

in vec3 pos;
flat in int instanceID;

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
    float T = (-b - sqrt((b*b) - 4.0*a*c))/(2.0*a);
    if (T < 0.0) {
        return -1.0;
    }
    return T;
}

void main() {
    vec3 dir = normalize(pos - u_camPos);
    float t = raySphereIntersect(u_camPos, dir, u_pos[instanceID], 1);

    if (t == -1) {
        //fragColour = vec4(0.);
        discard;
    }
    else {
        vec3 p = u_camPos + t * dir;
        vec3 normal = p - u_pos[instanceID];

        vec3 dir2 = reflect(dir, normal);

        float minT = 1000000;
        int minTID;
        for (int i = 0; i < u_num; i++) {
            if (i == instanceID) {
                continue;
            }
            float t2 = raySphereIntersect(p, dir2, u_pos[i], 1);
            if (t2 == -1) {
                continue;
            }
            else if (t2 < minT) {
                minT = t2;
                minTID = i;
            }
        }

        vec3 p2 = p + minT * dir2;
        vec3 normal2 = p2 - u_pos[minTID];

        if (minT == 1000000) {
            fragColour = texture(u_sky, dir2);
        }
        else {
            fragColour = texture(u_sky, normal2);
        }

        /*vec4 depth_vec = u_view * u_proj * vec4(pos, 1.0);
        float depth = ((depth_vec.z / depth_vec.w) + 1.0) * 0.5; 
        gl_FragDepth = depth;*/

        vec4 clipPos = u_view * vec4(u_camPos, 1.0);
        float ndcDepth = clipPos.z / clipPos.w;
        gl_FragDepth = ((gl_DepthRange.diff * ndcDepth) +
            gl_DepthRange.near + gl_DepthRange.far) / 2.0;
    }
}
