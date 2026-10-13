shader_type canvas_item;
render_mode unshaded;
uniform sampler2D frente : filter_linear;
uniform sampler2D direita : filter_linear;
uniform sampler2D esquerda : filter_linear;
uniform sampler2D cima : filter_linear;
uniform sampler2D baixo : filter_linear;
uniform float fov = 3.14159265;
uniform float escala = 1.0;
uniform float recorte = 1.0;
uniform float orientacao = 0.0;
uniform vec2 espelho = vec2(1.0);

void fragment() {
    vec2 p = (UV * 2.0 - 1.0) * espelho / escala;
    p = mat2(vec2(cos(orientacao), sin(orientacao)), vec2(-sin(orientacao), cos(orientacao))) * p;
    float r = length(p);
    vec3 cor = vec3(0.0);
    if (r <= recorte) {
        float t = r * fov * 0.5;
        vec3 d = vec3(p.x * sin(t) / max(r, 0.00001), -p.y * sin(t) / max(r, 0.00001), -cos(t));
        vec3 a = abs(d);
        vec2 uv;
        if (a.z >= a.x && a.z >= a.y) {
            uv = vec2(d.x, -d.y) / max(a.z, 0.00001) * 0.5 + 0.5;
            cor = texture(frente, uv).rgb;
        } else if (a.x >= a.y) {
            uv = vec2(d.x > 0.0 ? d.z : -d.z, -d.y) / a.x * 0.5 + 0.5;
            cor = d.x > 0.0 ? texture(direita, uv).rgb : texture(esquerda, uv).rgb;
        } else {
            uv = vec2(d.x, d.y > 0.0 ? -d.z : d.z) / a.y * 0.5 + 0.5;
            cor = d.y > 0.0 ? texture(cima, uv).rgb : texture(baixo, uv).rgb;
        }
    }
    COLOR = vec4(cor, 1.0);
}
