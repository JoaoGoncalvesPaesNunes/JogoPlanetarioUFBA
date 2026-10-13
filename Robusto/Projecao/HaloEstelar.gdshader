shader_type spatial;
render_mode unshaded, blend_add, cull_back, depth_draw_never, shadows_disabled;
uniform vec3 cor : source_color = vec3(0.54, 0.72, 0.95);
uniform float intensidade : hint_range(0.0, 0.5) = 0.2;
void fragment() {
    float face = clamp(dot(normalize(NORMAL), normalize(VIEW)), 0.0, 1.0);
    float raio = sqrt(max(0.0, 1.0 - face * face));
    float halo = smoothstep(0.76, 0.84, raio) * (1.0 - smoothstep(0.84, 1.0, raio));
    ALBEDO = cor;
    ALPHA = halo * intensidade;
}
