shader_type spatial;
render_mode unshaded;
uniform vec3 cor_quente : source_color = vec3(0.86, 0.93, 1.0);
uniform vec3 cor_profunda : source_color = vec3(0.30, 0.48, 0.66);
uniform float emissao : hint_range(0.0, 1.5) = 0.65;
uniform float contraste : hint_range(0.1, 1.0) = 0.7;
uniform float velocidade_fluxo : hint_range(0.0, 0.05) = 0.012;
varying vec3 superficie;

float hash3(vec3 p) {
    p = fract(p * 0.1031);
    p += dot(p, p.yzx + 33.33);
    return fract((p.x + p.y) * p.z);
}
float ruido(vec3 p) {
    vec3 i = floor(p), f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    return mix(mix(mix(hash3(i), hash3(i+vec3(1,0,0)), f.x),
                   mix(hash3(i+vec3(0,1,0)), hash3(i+vec3(1,1,0)), f.x), f.y),
               mix(mix(hash3(i+vec3(0,0,1)), hash3(i+vec3(1,0,1)), f.x),
                   mix(hash3(i+vec3(0,1,1)), hash3(i+vec3(1,1,1)), f.x), f.y), f.z);
}
float camadas(vec3 p) {
    return ruido(p) * 0.58 + ruido(p * 2.07 + 7.3) * 0.28 + ruido(p * 4.13 + 14.1) * 0.14;
}
void vertex() { superficie = normalize(VERTEX); }
void fragment() {
    vec3 p = normalize(superficie);
    float tempo = TIME * velocidade_fluxo;
    float macro = camadas(p * 8.0 + vec3(tempo, 0.0, -tempo));
    float micro = camadas(p * 46.0 + vec3(macro * 2.5, tempo * 0.4, 0.0));
    float fluxo = smoothstep(0.24, 0.76, macro * 0.62 + micro * 0.38);
    float filamentos = pow(1.0 - abs(micro * 2.0 - 1.0), 8.0) * 0.10;
    float detalhe = clamp(mix(0.65, fluxo, contraste) + filamentos, 0.0, 1.0);
    float face = clamp(dot(normalize(NORMAL), normalize(VIEW)), 0.0, 1.0);
    float limbo = 0.64 + 0.36 * pow(face, 0.35);
    vec3 cor = mix(cor_profunda, cor_quente, detalhe);
    ALBEDO = cor * limbo * 0.68;
    EMISSION = cor * (0.30 + detalhe * 0.44) * emissao * limbo;
}
