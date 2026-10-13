shader_type spatial;
render_mode unshaded, cull_disabled, depth_draw_never;

void fragment() {
    float incidencia = abs(dot(normalize(NORMAL), normalize(VIEW)));
    float borda = pow(1.0 - incidencia, 5.0);
    ALBEDO = vec3(0.45, 0.67, 0.69);
    // Sem refracao nem reflexos fortes que ocultem o panorama na projecao.
    ALPHA = 0.012 + borda * 0.065;
}
