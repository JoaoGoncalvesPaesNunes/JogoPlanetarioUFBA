shader_type spatial;
render_mode specular_schlick_ggx;

uniform bool gasoso = false;
uniform vec3 cor_oceano : source_color = vec3(0.025, 0.15, 0.26);
uniform vec3 cor_terra : source_color = vec3(0.2, 0.32, 0.17);
varying vec3 esfera;

float ruido(vec3 p) {
    vec3 i = floor(p);
    vec3 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    vec3 passo = vec3(1.0, 57.0, 113.0);
    float n = dot(i, passo);
    return mix(mix(mix(fract(sin(n) * 43758.5453), fract(sin(n + 1.0) * 43758.5453), f.x),
                   mix(fract(sin(n + 57.0) * 43758.5453), fract(sin(n + 58.0) * 43758.5453), f.x), f.y),
               mix(mix(fract(sin(n + 113.0) * 43758.5453), fract(sin(n + 114.0) * 43758.5453), f.x),
                   mix(fract(sin(n + 170.0) * 43758.5453), fract(sin(n + 171.0) * 43758.5453), f.x), f.y), f.z);
}

float terreno(vec3 p) {
    float valor = 0.0;
    float amplitude = 0.5;
    for (int i = 0; i < 5; i++) {
        valor += ruido(p) * amplitude;
        p = p * 2.03 + vec3(4.1, 7.2, 1.3);
        amplitude *= 0.5;
    }
    return valor;
}

void vertex() { esfera = normalize(VERTEX); }

void fragment() {
    vec3 p = normalize(esfera);
    float altura = terreno(p * 4.5 + vec3(2.0, 0.0, 5.0));
    if (gasoso) {
        float bandas = sin(p.y * 65.0 + terreno(p * 7.0) * 8.0) * 0.5 + 0.5;
        ALBEDO = mix(vec3(0.22, 0.14, 0.09), vec3(0.76, 0.62, 0.41), bandas * 0.65 + altura * 0.35);
        ROUGHNESS = 0.95;
    } else {
        float continente = smoothstep(0.48, 0.51, altura);
        vec3 terra = mix(cor_terra, vec3(0.48, 0.41, 0.27), smoothstep(0.55, 0.72, altura));
        vec3 oceano = mix(cor_oceano, vec3(0.08, 0.35, 0.40), smoothstep(0.39, 0.49, altura));
        ALBEDO = mix(oceano, terra, continente);
        float gelo = smoothstep(0.85, 0.96, abs(p.y) + altura * 0.075);
        ALBEDO = mix(ALBEDO, vec3(0.84, 0.89, 0.86), gelo);
        float nuvens = smoothstep(0.52, 0.67, terreno(p * 9.0 + vec3(TIME * 0.004, 13.0, 0.0)));
        ALBEDO = mix(ALBEDO, vec3(0.88, 0.92, 0.91), nuvens * 0.92);
        ROUGHNESS = mix(0.3, 0.85, max(continente, nuvens));
        SPECULAR = 0.4;
    }
}
