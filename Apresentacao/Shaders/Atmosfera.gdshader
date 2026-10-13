shader_type spatial;
render_mode unshaded, blend_add, cull_back, depth_draw_never;
uniform vec3 cor : source_color = vec3(0.16, 0.5, 0.95);
void fragment() {
    float borda = pow(1.0 - clamp(dot(normalize(NORMAL), normalize(VIEW)), 0.0, 1.0), 3.5);
    ALBEDO = cor;
    ALPHA = borda * 0.42;
}
