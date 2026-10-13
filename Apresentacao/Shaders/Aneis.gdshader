shader_type spatial;
render_mode cull_disabled;
void fragment() {
    float faixa = UV.x;
    float filamentos = sin(faixa * 260.0) * 0.1 + sin(faixa * 87.0) * 0.15;
    float divisao = smoothstep(0.015, 0.03, abs(faixa - 0.66));
    ALBEDO = mix(vec3(0.28, 0.23, 0.16), vec3(0.72, 0.65, 0.49), 0.55 + filamentos);
    ROUGHNESS = 1.0;
    ALPHA = smoothstep(0.0, 0.05, faixa) * (1.0 - smoothstep(0.94, 1.0, faixa)) * divisao * 0.85;
}
